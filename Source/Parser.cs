using Pidgin;
using System;
using Pidgin.Expression;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using Small_lang.AST;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ExpOperator = Pidgin.Expression.Operator;

namespace Small_lang
{
    public static class LangParser
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

        static Parser<char, Func<BaseAST, BaseAST>> Unary(Parser<char, ASTType> op)
            => op.Select<Func<BaseAST, BaseAST>>(op => hs => new UnaryExprAST(op, hs));
        
        static Parser<char, Func<BaseAST, BaseAST, BaseAST>> Binary(Parser<char, ASTType> op)
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
            Percent = Tok('%'),
            LessThan = Tok('<'),
            Exclamation = Tok('!'),
            GreaterThan = Tok('>');

        private static Parser<char, string>
            Assign = Tok(":="),
            Fn = Tok("func"),
            Arrow = Tok("->"),
            LessEqual = Tok("<="),
            GreaterEqual = Tok(">="),
            Equal = Tok("=="),
            NotEqual = Tok("!="),
            Ampersand = Tok("&&"),
            VerticalBar = Tok("||"),
            If = Tok("if"),
            Elif = Tok("elif"),
            Else = Tok("else"),
            True = Tok("true"),
            False = Tok("false"),
            Int = Tok("Int"),
            Float = Tok("Float"),
            Bool = Tok("Bool"),
            Unit = Tok("Unit"),
            Then = Tok("then"),
            End = Tok("end"),
            Read = Tok("read"),
            Write = Tok("write"),
            Repeat = Tok("repeat"),
            Until = Tok("until"),
            For = Tok("for"),
            Break = Tok("break"),
            Continue = Tok("continue"),
            Begin = Tok("begin");

        static Parser<char, Unit> NonKeyWords = Not(OneOf(
                Try(Fn), If, Try(Elif), Else, Try(True), False, Int, Bool, Float, Unit, 
                Then, End, Read, Write, Repeat, Until, Try(Break), Continue
            ));


        static Parser<char, string>
            Ident = Lookahead(NonKeyWords)
                .Then(
                    Tok(Letter.Then(LetterOrDigit.Or(Lodash).ManyString(), (h, t) => h + t))
                );

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

        #region Unary / Binary operations
        static readonly Parser<char, Func<BaseAST, BaseAST, BaseAST>>
            Add = Binary(Plus.ThenReturn(ASTType.Add)),
            Sub = Binary(Minus.ThenReturn(ASTType.Subtract)),
            Mul = Binary(Times.ThenReturn(ASTType.Multiply)),
            Div = Binary(Divide.ThenReturn(ASTType.Divide)),
            Mod = Binary(Percent.ThenReturn(ASTType.Modulo)),
            And = Binary(Ampersand.ThenReturn(ASTType.And)),
            Or = Binary(VerticalBar.ThenReturn(ASTType.Or)),
            LT = Binary(LessThan.ThenReturn(ASTType.LessThan)),
            LE = Binary(LessEqual.ThenReturn(ASTType.LessEqual)),
            GT = Binary(GreaterThan.ThenReturn(ASTType.GreaterThan)),
            GE = Binary(GreaterEqual.ThenReturn(ASTType.GreaterEqual)),
            NE = Binary(NotEqual.ThenReturn(ASTType.NotEqual)),
            Eq = Binary(Equal.ThenReturn(ASTType.Equal));

        private static readonly Parser<char, Func<BaseAST, BaseAST>>
            Neg = Unary(Minus.ThenReturn(ASTType.Neg)),
            Not = Unary(Exclamation.ThenReturn(ASTType.Not));
        
        #endregion

        #region Parsers

        static Parser<char, TypedArg> PTypedArg =
            from ident in Ident
            from _ in Colon
            from ty in Type
            select new TypedArg(ident, ty);

        static Parser<char, BlockExprAST> PBody =
            Body(
                Rec(() => PNormalStmt).Many()
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
            PBreak = Break.ThenReturn<BaseAST>(new BreakStmtAST()),
            PContinue = Continue.ThenReturn<BaseAST>(new ContinueStmtAST()),
            
            POnlyIf =
                from cond in If.Then(Rec(() => PExpr))
                from _1 in Then
                from stmts in PNormalStmt.Many()
                from _2 in End
                select new IfStmtAST(new[] { new Branch(cond, new BlockExprAST(stmts)) }, null) as BaseAST,

            PIfElse =
                from cond in If.Then(Rec(() => PExpr))
                from _1 in Then
                from ifStmts in PNormalStmt.Many()
                from _2 in Else
                from elseStmts in PNormalStmt.Many()
                from _3 in End
                select new IfStmtAST(
                    new[] { new Branch(cond, new BlockExprAST(ifStmts)) },
                    new ElseBranch(new BlockExprAST(ifStmts))
                ) as BaseAST,

            PIfStmt = Try(PIfElse).Or(POnlyIf),

            PIdent = Ident
                .Select<BaseAST>(s => new VariableExprAST(s))
                .Labelled("identifier"),

            PInt = Tok(Num).Select<BaseAST>(elm => new IntExprAST(elm)),

            PFloat =
                from num in Num
                from _0 in Char('.')
                from rest in Digit.ManyString().Before(SkipWhitespaces)
                select new FloatExprAST(double.Parse($"{num}.{rest}")) as BaseAST,

            PTrue = True.ThenReturn(new BoolExprAST(true) as BaseAST),
            PFalse = False.ThenReturn(new BoolExprAST(false) as BaseAST),

            PRead = Read.Then(Ident).Select<BaseAST>(s => new ReadStmtAST(s, TypeKind.Int)),

            PWrite = Write.Then(Rec(() => PExpr)).Select<BaseAST>(v => new WriteStmtAST(v)),

            PRepeat = 
                from _1 in Repeat
                from stmts in PNormalStmt.AtLeastOnce()
                from _2 in Until
                from cond in PNormalExpr
                select new RepeatStmtAST(new Branch(cond, new BlockExprAST(stmts))) as BaseAST,
            
            PFor = 
                from _1 in For
                from exprs in OneOf(
                    Try(PAssign), PNormalExpr, PRead, PWrite
                ).SeparatedAtLeastOnce(SemiColon).Select(e => e.ToList())
                from _2 in Begin
                from stmts in PNormalStmt.Many()
                from _3 in End
                select new ForStmtAST(
                    new Branch(exprs[1], new BlockExprAST(stmts)), 
                    exprs[0], 
                    exprs[2]
                ) as BaseAST,

            PLit = OneOf(
                    Try(PFloat),
                    PInt,
                    PTrue,
                    PFalse
                )
                .Labelled("literal"),

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
                            .And(ExpOperator.InfixL(Div))
                            .And(ExpOperator.InfixL(Mod)),
                        ExpOperator.InfixL(Add)
                            .And(ExpOperator.InfixL(Sub)),
                        ExpOperator.Unary(UnaryOperatorType.Prefix, Neg)
                            .And(ExpOperator.Unary(UnaryOperatorType.Prefix, Not)),
                        ExpOperator.InfixN(GE)
                            .And(ExpOperator.InfixN(GT))
                            .And(ExpOperator.InfixN(LE))
                            .And(ExpOperator.InfixN(LT)),
                        ExpOperator.InfixL(Eq)
                            .And(ExpOperator.InfixL(NE)),
                        ExpOperator.InfixL(And),
                        ExpOperator.InfixL(Or)
                    }
                )
            ).Labelled("expression"),

            TopFieldOnlyStmt = OneOf(
                Try(PFunc),
                Proto
            ),

            PExpr = OneOf(
               // Try(PIfStmt), if expr is decrypted
               PNormalExpr
            ),

            PNormalStmt = OneOf(
                PIfStmt,
                Try(PRead).Before(Delimiter),
                PRepeat,
                PFor,
                PBreak.Before(Delimiter),
                PContinue.Before(Delimiter),
                PWrite.Before(Delimiter),
                Try(PAssign).Before(Delimiter),
                Try(PExpr.Before(Delimiter))
            ),
        
            Stmt = TopFieldOnlyStmt.Or(PNormalStmt);

        static Parser<char, IEnumerable<BaseAST>> Program = Stmt.AtLeastOnce();

        #endregion

        #region Entry
        public static List<BaseAST> ParseAll(string input) => Program.ParseOrThrow(
            Regex.Replace(input.Trim(), @"//.*?\r?\n", "")
        ).ToList();

        #endregion

    }
}
