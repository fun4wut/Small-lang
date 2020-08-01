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
            var s = "a2a*(133+2.3);";
            var expectecd = new List<ExprAST>
            {
                new BinaryExprAST(
                    ExprType.MultiplyExpr,
                    new VariableExprAST("a2a"),
                    new BinaryExprAST(
                        ExprType.AddExpr,
                        new IntExprAST(133),
                        new FloatExprAST(2.3)
                    )
                )
            };
            Assert.AreEqual(expectecd, LangParser.ParseOrThrow(s));
        }

        [Test]
        public void TestDemo()
        {
            Assert.IsTrue(2.3 / 1 - 2.3 == 0);
        }


    }
}