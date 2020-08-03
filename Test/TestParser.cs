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
        public void Assignment()
        {
            var s = "let a12 = a*31;";
            var expected = new List<ExprAST>
            {
                new AssignExprAST(
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
            var s = "func xyz(a: Int, b: Float);";
            var expected = new List<ExprAST>
            {
                new ProtoExprAST(
                    "xyz",
                    new List<TypedArg>
                    {
                        new TypedArg ("a", TypeEnum.Int),
                        new TypedArg ("b", TypeEnum.Float)
                    }
                )
            };
            Assert.AreEqual(expected, LangParser.ParseAll(s));
        }

        [Test]
        public void FuncDef()
        {
            var s = "func xyz(a: Int, b: Float) { 1; };";
            var proto = new ProtoExprAST(
                "xyz",
                new List<TypedArg>
                {
                    new TypedArg ("a", TypeEnum.Int),
                    new TypedArg ("b", TypeEnum.Float)
                }
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
    }
}