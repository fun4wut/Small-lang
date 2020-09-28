using Pidgin;
using System;
using Pidgin.Expression;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using Small_lang.AST;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExpOperator = Pidgin.Expression.Operator;

namespace Small_lang
{
    public sealed partial class LangParser
    {
        #region Helper methods
        static Parser<char, T> Tok<T>(Parser<char, T> p) => Try(p).Before(SkipWhitespaces);
        static Parser<char, char> Tok(char value) => Tok(Char(value));
        static Parser<char, string> Tok(string value) => Tok(String(value));
        static Parser<char, T> Parenthesised<T>(Parser<char, T> parser) => parser.Between(LBracket, RBracket);
        static Parser<char, T> Body<T>(Parser<char, T> parser) => parser.Between(LBrace, RBrace);
        static Parser<char, Branch> Branch(Parser<char, string> parser) =>
            parser.Then(Rec(() => PExpr)).Then(PBody, (cond, actions) => new Branch(cond, actions));

        static Parser<char, Func<BaseAST, BaseAST>> Call(Parser<char, BaseAST> subExpr)
            => Parenthesised(subExpr.Separated(Comma))
                .Select<Func<BaseAST, BaseAST>>(args => exp => exp switch
                    {
                        VariableExprAST ident => new CallExprAST(ident.Name, args),
                        _ => throw new Exception("callee must be ident"),
                    }
                )
                .Labelled("function call");

        private static Parser<char, Func<BaseAST, BaseAST, BaseAST>> Binary(Parser<char, ASTType> op)
            => op.Select<Func<BaseAST, BaseAST, BaseAST>>(op => (l, r) => new BinaryExprAST(op, l, r));
        #endregion

        #region Tokens
        static Parser<char, char>
            SemiColon = Tok(';'),
            Comma = Tok(','),
            Lodash = Tok('_'),
            Colon = Tok(':'),
            LBracket = Tok('('),
            RBracket = Tok(')'),
            LBrace = Tok('{'),
            RBrace = Tok('}'),
            Plus = Tok('+'),
            Minus = Tok('-'),
            Times = Tok('*'),
            Divide = Tok('/'),
            LessThan = Tok('<'),
            GreaterThan = Tok('>'),
            Assign = Tok('=');

        static Parser<char, string>
            Fn = Tok("func"),
            Arrow = Tok("->"),
            LessEqual = Tok("<="),
            GreaterEqual = Tok(">="),
            Equal = Tok("=="),
            If = Tok("if"),
            Elif = Tok("elif"),
            Else = Tok("else"),
            True = Tok("true"),
            False = Tok("false"),
            Let = Tok("let"),
            Mut = Tok("mut"),
            Int = Tok("Int"),
            Float = Tok("Float"),
            Bool = Tok("Bool"),
            Unit = Tok("Unit");

        static Parser<char, Unit> NonKeyWords = Not(OneOf(
                Try(Fn), If, Try(Elif), Else, True, False, Let, Mut, Int, Bool, Float, Unit
            ));


        static Parser<char, string> Ident = Lookahead(NonKeyWords)
                .Then(
                    Tok(Letter.Then(LetterOrDigit.Or(Lodash).ManyString(), (h, t) => h + t))
                );

        static Parser<char, ASTType>
            LetTy = Let.ThenReturn(ASTType.Let),
            MutTy = Mut.ThenReturn(ASTType.Mut);

        static Parser<char, TypeKind>
            IntTy = Int.ThenReturn(TypeKind.Int),
            FloatTy = Float.ThenReturn(TypeKind.Float),
            BoolTy = Bool.ThenReturn(TypeKind.Bool),
            UnitTy = Unit.ThenReturn(TypeKind.Unit),
            Type = OneOf(
                IntTy,
                FloatTy,
                BoolTy,
                UnitTy
            );

        static Parser<char, Unit> Delimiter = SemiColon.SkipAtLeastOnce().Then(EndOfLine.SkipMany());

        #endregion

        #region Binary operations
        static readonly Parser<char, Func<BaseAST, BaseAST, BaseAST>>
            Add = Binary(Plus.ThenReturn(ASTType.Add)),
            Sub = Binary(Minus.ThenReturn(ASTType.Subtract)),
            Mul = Binary(Times.ThenReturn(ASTType.Multiply)),
            Div = Binary(Divide.ThenReturn(ASTType.Divide)),
            LT = Binary(LessThan.ThenReturn(ASTType.LessThan)),
            LE = Binary(LessEqual.ThenReturn(ASTType.LessEqual)),
            GT = Binary(GreaterThan.ThenReturn(ASTType.GreaterThan)),
            GE = Binary(GreaterEqual.ThenReturn(ASTType.GreaterEqual)),
            Eq = Binary(Equal.ThenReturn(ASTType.Equal));
        #endregion

        #region Parsers

        static Parser<char, TypedArg> PTypedArg =
            from ident in Ident
            from _ in Colon
            from ty in Type
            select new TypedArg(ident, ty);

        static Parser<char, BlockExprAST> PBody =
            Body(
                Rec(() => NormalStmt).Many()
                .Then(Rec(() => PExpr).Optional(), (elms, ret) => 
                    new BlockExprAST(
                        ret.Match(
                            just: val => elms.Append(val),
                            // if expr is the special case
                            nothing: () => elms.Any() && elms.Last().NodeType != ASTType.If
                                ? elms.Append(new NopStmt())
                                : elms
                    ))
                )
            );

        static Parser<char, BaseAST>
            PIfExpr = 
                from @if in Branch(If)
                from elif in Branch(Elif).Many()
                from @else in Tok("else").Then(PBody, (_, actions) => new ElseBranch(actions)).Optional()
                select new IfExprAST(
                    elif.Prepend(@if),
                    @else.Match(
                        just: elm => elm,
                        nothing: () => null
                    )
                ) as BaseAST,

            PIdent = Ident
                .Select<BaseAST>(s => new VariableExprAST(s))
                .Labelled("identifier"),

            PInt = Tok(Num).Select<BaseAST>(elm => new IntExprAST(elm)),

            PFloat =
                from num in Num
                from _0 in Char('.')
                from rest in Digit.ManyString().Before(SkipWhitespaces)
                select new FloatExprAST(double.Parse($"{num}.{rest}")) as BaseAST ,

            PTrue = True.ThenReturn(new BoolExprAST(true) as BaseAST),
            PFalse = False.ThenReturn(new BoolExprAST(false) as BaseAST),

            PLit = OneOf(
                    Try(PFloat),
                    PInt,
                    PTrue,
                    PFalse
                )
                .Labelled("literial"),

            PAssign = 
                from ident in Ident
                from _0 in Assign
                from val in PExpr
                select new AssignStmtAST(ident, val) as BaseAST,


            Proto =
                from _0 in Fn
                from ident in Ident
                from args in Parenthesised(PTypedArg.Separated(Comma))
                from _1 in Arrow
                from ty in Type
                select new ProtoStmtAST(ident, args, ty) as BaseAST,

            PFunc = Proto.Then(
                PBody,
                (proto, exprs) =>
                    {
                        var _proto = proto as ProtoStmtAST;
                        return new FuncStmtAST(_proto!, exprs) as BaseAST;
                    }
            ),

            PNormalExpr = ExpressionParser.Build<char, BaseAST>(
                expr => (
                    OneOf(
                        PLit,
                        PIdent,
                        Parenthesised(expr).Labelled("parenthesised expression")
                    ),
                    new[]
                    {
                        ExpOperator.Postfix(Call(expr)),
                        ExpOperator.InfixL(Mul)
                            .And(ExpOperator.InfixL(Div)),
                        ExpOperator.InfixL(Add)
                            .And(ExpOperator.InfixL(Sub)),
                        ExpOperator.InfixN(GE)
                            .And(ExpOperator.InfixN(GT))
                            .And(ExpOperator.InfixN(LE))
                            .And(ExpOperator.InfixN(LT)),
                        ExpOperator.InfixL(Eq)
                    }
                )
            ).Labelled("expression"),

            TopFieldOnlyStmt = OneOf(
                Try(PFunc),
                Proto
            ),

            PExpr = OneOf(
               Try(PIfExpr),
               PNormalExpr
            ),

            NormalStmt = OneOf(
                PIfExpr,
                Try(PAssign).Before(Delimiter),
                Try(PExpr.Before(Delimiter))
            ),
        
            Stmt = TopFieldOnlyStmt.Or(NormalStmt);

        static Parser<char, IEnumerable<BaseAST>> Program = Stmt.Many();

        #endregion

        #region Entry
        public static List<BaseAST> ParseAll(string input) => Program.ParseOrThrow(input.Trim()).ToList();

        public static BaseAST ParseSingle(string input) => Stmt.ParseOrThrow(input.Trim());
        #endregion

    }
}
