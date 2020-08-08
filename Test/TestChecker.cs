using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Kumiko_lang.AST;
using Kumiko_lang;

namespace Test
{
    class TestChecker
    {
        TypeChecker checker = new TypeChecker();

        [Test]
        public void BinOp_Lhs_Rhs_Not_Match()
        {
            var s = @"4 == 3.5 < 3;";
            Assert.Throws<TypeCheckException>(() => checker.Check(LangParser.ParseAll(s)));
        }
    }
}
