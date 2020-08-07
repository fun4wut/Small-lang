using NUnit.Framework;
using Kumiko_lang;
using Kumiko_lang.AST;
using System.Collections.Generic;
using System;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
namespace Test
{
    public class ParserTests
    {

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void SingleLine()
        {
            var s = "a2a*(133+2.3);12;";
            var expected = new List<ExprAST>
            {
                new BinaryExprAST(
                    ExprType.MultiplyExpr,
                    new VariableExprAST("a2a"),
                    new BinaryExprAST(
                        ExprType.AddExpr,
                        new IntExprAST(133),
                        new FloatExprAST(2.3)
                    )
                ),
                new IntExprAST(12)
            };
            Assert.AreEqual(expected, LangParser.ParseAll(s));
        }

        [Test]
        public void MultiLine()
        {
            var s = @"
a;;;


12;";
            var expected = new List<ExprAST>
            {
                new VariableExprAST("a"),
                new IntExprAST(12)
            };
            Assert.AreEqual(expected, LangParser.ParseAll(s));
        }

        [Test]
        public void ImmutVar()
        {
            var s = "let a12 = a*31;";
            var expected = new List<ExprAST>
            {
                new DeclExprAST(
                    ExprType.LetExpr,
                    "a12",
                    new BinaryExprAST(
                        ExprType.MultiplyExpr,
                        new VariableExprAST("a"),
                        new IntExprAST(31)
                    )
                )
            };
            Assert.AreEqual(expected, LangParser.ParseAll(s));
        }

        [Test]
        public void Prototype()
        {
            var s = "func xyz(a: Int, b: Float) -> Float;";
            var expected = new List<ExprAST>
            {
                new ProtoExprAST(
                    "xyz",
                    new List<TypedArg>
                    {
                        new TypedArg ("a", TypeEnum.Int),
                        new TypedArg ("b", TypeEnum.Float)
                    },
                    TypeEnum.Float
                )
            };
            Assert.AreEqual(expected, LangParser.ParseAll(s));
        }

        [Test]
        public void FuncDef()
        {
            var s = "func xyz(a: Int, b: Float) -> Int { 1; };";
            var proto = new ProtoExprAST(
                "xyz",
                new List<TypedArg>
                {
                    new TypedArg ("a", TypeEnum.Int),
                    new TypedArg ("b", TypeEnum.Float)
                },
                TypeEnum.Int
            );
            var expected = new List<ExprAST>
            {
                new FuncExprAST(
                    proto,
                    new List<ExprAST>
                    {
                        new IntExprAST(1)
                    }
                )
            };
            Assert.AreEqual(expected, LangParser.ParseAll(s));
        }

        [Test]
        public void Call()
        {
            var s = "ab(2.0, 3);";
            var expected = new List<ExprAST>
            {
                new CallExprAST(
                    "ab",
                    new List<ExprAST>
                    {
                        new FloatExprAST(2.0),
                        new IntExprAST(3)
                    }
                )
            };
            Assert.AreEqual(expected, LangParser.ParseAll(s));
        }

        [Test]
        public void Call_Throw()
        {
            var s = "1(2, 3);";
            Assert.Throws<Exception>(() => LangParser.ParseAll(s));
        }

        [Test]
        public void MutVar()
        {
            var s = "mut a = 3;";
            var expected = new List<ExprAST>
            {
                new DeclExprAST(
                    ExprType.MutExpr,
                    "a",
                    new IntExprAST(3)
                )
            };
            Assert.AreEqual(expected, LangParser.ParseAll(s));
        }

        [Test]
        public void Assign()
        {
            var s = "a = 3;";
            var expected = new List<ExprAST>
            {
                new AssignExprAST(
                    "a",
                    new IntExprAST(3)
                )
            };
            Assert.AreEqual(expected, LangParser.ParseAll(s));
        }

        [Test]
        public void Compare()
        {
            var s = "a >= 3;";
            var expected = new List<ExprAST>
            {
                new BinaryExprAST(
                    ExprType.GreatEqualExpr,
                    new VariableExprAST("a"),
                    new IntExprAST(3)
                )
            };
            Assert.AreEqual(expected, LangParser.ParseAll(s));
        }

        [Test]
        public void If()
        {
            var s = "if 2<3 {};";
            var expected = new List<ExprAST>
            {
                new BinaryExprAST(
                    ExprType.GreatEqualExpr,
                    new VariableExprAST("a"),
                    new IntExprAST(3)
                )
            };
            Assert.AreEqual(expected, LangParser.ParseAll(s));
        }
    }
}