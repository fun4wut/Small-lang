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
        private static Parser<char, Func<ExprAST, ExprAST, ExprAST>> Binary(Parser<char, ExprType> op)
            => op.Select<Func<ExprAST, ExprAST, ExprAST>>(op => (l, r) => new BinaryExprAST(op, l, r));
        #endregion

        #region Char tokens
        static Parser<char, char>
            SemiColon = Tok(';'),
            LBracket = Tok('('),
            RBracket = Tok(')'),
            Plus = Tok('+'),
            Minus = Tok('-'),
            Times = Tok('*'),
            Divide = Tok('/'),
            Assign = Tok('=');
        #endregion

        #region Binary operations
        static readonly Parser<char, Func<ExprAST, ExprAST, ExprAST>>
            Add = Binary(Plus.ThenReturn(ExprType.AddExpr)),
            Sub = Binary(Minus.ThenReturn(ExprType.SubtractExpr)),
            Mul = Binary(Times.ThenReturn(ExprType.MultiplyExpr)),
            Div = Binary(Divide.ThenReturn(ExprType.DivideExpr));
        #endregion

        #region Parsers
        static Parser<char, ExprAST> Ident =
            Tok(Letter.Then(LetterOrDigit.ManyString(), (h, t) => h + t))
                .Select<ExprAST>(s => new VariableExprAST(s))
                .Labelled("identifier");

        static Parser<char, ExprAST> Lit =
            Tok(Real)
                .Select(elm => elm.ToString().Contains('.')
                    ? new FloatExprAST(elm) as ExprAST 
                    : new IntExprAST((int)elm) as ExprAST 
                )
                .Labelled("literial");

        static Parser<char, ExprAST> Expr = ExpressionParser.Build<char, ExprAST>(
            expr => ( 
                OneOf(
                    Lit,
                    Ident,
                    Parenthesised(expr).Labelled("parenthesised expression")
                ),
                new[]
                {
                    ExpOperator.InfixL(Mul)
                        .And(ExpOperator.InfixL(Div)),
                    ExpOperator.InfixL(Add)
                        .And(ExpOperator.InfixL(Sub)),
                }
            )
        ).Labelled("expression");

        static Parser<char, IEnumerable<ExprAST>> Program = Expr.Before(SemiColon).Many();

        #endregion

        #region Entry
        public static List<ExprAST> ParseOrThrow(string input) => Program.ParseOrThrow(input).ToList();
        #endregion

    }
}
