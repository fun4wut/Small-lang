using NUnit.Framework;
using Kumiko_lang;
using Kumiko_lang.AST;
using System.Collections.Generic;
using System;

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
            Assert.AreEqual(expected, LangParser.ParseOrThrow(s));
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
            Assert.AreEqual(expected, LangParser.ParseOrThrow(s));
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
            Assert.AreEqual(expected, LangParser.ParseOrThrow(s));
        }
    }
}