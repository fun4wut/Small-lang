using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Kumiko_lang.AST;
using Kumiko_lang;
using Kumiko_lang.TypeCheck;
namespace Test
{
    class TestChecker
    {
        TypeChecker checker = new TypeChecker();

        private void CheckIt(string s) => checker.ReorderAndCheck(LangParser.ParseAll(s));

        [SetUp]
        public void Setup()
        {
            checker = new TypeChecker();
        }

        [Test]
        public void BinOp_Lhs_Rhs_Not_Match()
        {
            var s = @"4 == 3.5 < 3;";
            Assert.Throws<TypeCheckException>(() => CheckIt(s));
        }

        [Test]
        public void Var_Func_Same_Name()
        {
            var s = "func ab() -> Int\n let ab = 1;";
            Assert.Throws<TypeCheckException>(() => CheckIt(s));
            s = "let ab = 1;func ab() -> Int";
            Assert.Throws<TypeCheckException>(() => CheckIt(s));
        }

        [Test]
        public void Func_Func_Same_Name()
        {
            var s = "func ab() -> Int\n func ab() -> Float;";
            Assert.Throws<TypeCheckException>(() => CheckIt(s));
            s = "func ab() -> Int\n func ab() -> Int {4;}\n";
            Assert.DoesNotThrow(() => CheckIt(s));
        }

        [Test]
        public void Arg_Var_Same_Name()
        {
            var s = "func ab(a: Int) -> Int\n let a = 233;";
            Assert.DoesNotThrow(() => CheckIt(s));
        }

        [Test]
        public void Arg_Func_Same_Name()
        {
            var s = "func a(a: Int) -> Int";
            Assert.Throws<TypeCheckException>(() => CheckIt(s));
        }

        [Test]
        public void Var_Var_Same_Name()
        {
            var s = "let a = -5; let a = 10;";
            Assert.Throws<TypeCheckException>(() => CheckIt(s));
        }

        [Test]
        public void UndefinedVar()
        {
            var s = "a+1;";
            Assert.Throws<TypeCheckException>(() => CheckIt(s));
        }

    }
}
