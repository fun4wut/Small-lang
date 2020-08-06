using Pidgin;
using System;
using Pidgin.Expression;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using Kumiko_lang.AST;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExpOperator = Pidgin.Expression.Operator;

namespace Kumiko_lang
{
    public sealed partial class LangParser
    {
        #region Helper methods
        static Parser<char, T> Tok<T>(Parser<char, T> p) => Try(p).Before(SkipWhitespaces);
        static Parser<char, char> Tok(char value) => Tok(Char(value));
        static Parser<char, string> Tok(string value) => Tok(String(value));
        static Parser<char, T> Parenthesised<T>(Parser<char, T> parser) => parser.Between(LBracket, RBracket);
        static Parser<char, T> Body<T>(Parser<char, T> parser) => parser.Between(LBrace, RBrace);

        static Parser<char, Func<ExprAST, ExprAST>> Call(Parser<char, ExprAST> subExpr)
            => Parenthesised(subExpr.Separated(Comma))
                .Select<Func<ExprAST, ExprAST>>(args => exp => exp switch
                    {
                        VariableExprAST ident => new CallExprAST(ident.Name, args),
                        _ => throw new Exception("callee must be ident"),
                    }
                )
                .Labelled("function call");

        private static Parser<char, Func<ExprAST, ExprAST, ExprAST>> Binary(Parser<char, ExprType> op)
            => op.Select<Func<ExprAST, ExprAST, ExprAST>>(op => (l, r) => new BinaryExprAST(op, l, r));
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
            Assign = Tok('=');

        static Parser<char, string>
            Fn = Tok("func"),
            Arrow = Tok("->"),
            Ident = Tok(Letter.Then(LetterOrDigit.Or(Lodash).ManyString(), (h, t) => h + t));

        static Parser<char, ExprType>
            Let = Tok("let").ThenReturn(ExprType.LetExpr),
            Mut = Tok("mut").ThenReturn(ExprType.MutExpr);

        static Parser<char, TypeEnum>
            Int = Tok("Int").ThenReturn(TypeEnum.Int),
            Float = Tok("Float").ThenReturn(TypeEnum.Float),
            Bool = Tok("Bool").ThenReturn(TypeEnum.Bool),
            Unit = Tok("Unit").ThenReturn(TypeEnum.Unit),
            Type = OneOf(
                Int,
                Float,
                Bool,
                Unit
            );

        static Parser<char, Unit> Delimiter = SemiColon.SkipAtLeastOnce().Then(EndOfLine.SkipMany());

        #endregion

        #region Binary operations
        static readonly Parser<char, Func<ExprAST, ExprAST, ExprAST>>
            Add = Binary(Plus.ThenReturn(ExprType.AddExpr)),
            Sub = Binary(Minus.ThenReturn(ExprType.SubtractExpr)),
            Mul = Binary(Times.ThenReturn(ExprType.MultiplyExpr)),
            Div = Binary(Divide.ThenReturn(ExprType.DivideExpr));
        #endregion

        #region Parsers

        static Parser<char, TypedArg> PTypedArg =
            from ident in Ident
            from _ in Colon
            from ty in Type
            select new TypedArg(ident, ty);

        static Parser<char, ExprAST>
            PIdent = Ident
                .Select<ExprAST>(s => new VariableExprAST(s))
                .Labelled("identifier"),

            PLit = Tok(Real)
                .Select(elm => elm.ToString().Contains('.')
                    ? new FloatExprAST(elm) as ExprAST
                    : new IntExprAST((int)elm) as ExprAST
                )
                .Labelled("literial"),

            PDecl =
                from mutability in Let.Or(Mut)
                from ident in Ident
                from _1 in Assign
                from val in PNormalExpr
                select new DeclExprAST(mutability, ident, val) as ExprAST,

            Proto =
                from _0 in Fn
                from ident in Ident
                from args in Parenthesised(PTypedArg.Separated(Comma))
                from _1 in Arrow
                from ty in Type
                select new ProtoExprAST(ident, args, ty) as ExprAST,

            PFunc = Proto.Then(
                Body(Rec(() => NormalStmt).Before(Delimiter).Many()),
                (proto, exprs) =>
                    {
                        var _proto = proto as ProtoExprAST;
                        return new FuncExprAST(_proto!, exprs) as ExprAST;
                    }
            ),

            PNormalExpr = ExpressionParser.Build<char, ExprAST>(
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
                    }
                )
            ).Labelled("expression"),

            TopFieldOnlyStmt = OneOf(
                Try(PFunc),
                Proto
            ),

            NormalStmt = OneOf(
                PDecl,
                PNormalExpr
            ),
        
            Stmt = TopFieldOnlyStmt.Or(NormalStmt);

        static Parser<char, IEnumerable<ExprAST>> Program =
            TopFieldOnlyStmt.Or(NormalStmt.Before(Delimiter)).Many();

        #endregion

        #region Entry
        public static List<ExprAST> ParseAll(string input) => Program.ParseOrThrow(input.Trim()).ToList();

        public static ExprAST ParseSingle(string input) => Stmt.ParseOrThrow(input.Trim());
        #endregion

    }
}
