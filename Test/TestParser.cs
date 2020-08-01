using NUnit.Framework;
using Kumiko_lang;
using Kumiko_lang.AST;
using System.Collections.Generic;

namespace Test
{
    public class Tests
    {

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestSingle()
        {
            var s = "a*(1+2);";
            var expectecd = new List<ExprAST>
            {
                new BinaryExprAST(
                    ExprType.MultiplyExpr,
                    new VariableExprAST("a"),
                    new BinaryExprAST(
                        ExprType.AddExpr,
                        new NumberExprAST(1),
                        new NumberExprAST(2)
                    )
                )
            };
            Assert.AreEqual(expectecd, LangParser.ParseOrThrow(s));
        }
    }
}