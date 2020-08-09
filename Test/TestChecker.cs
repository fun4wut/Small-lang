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

        [Test]
        public void Bool_Op()
        {
            var s = "4.0 == 2 + 3;";
            Assert.DoesNotThrow(() => CheckIt(s));
            s = "4 < 2 == 4;";
            Assert.Throws<TypeCheckException>(() => CheckIt(s));
        }

        [Test]
        public void If_Else_Expr()
        {
            var s = @"mut a = if 2 == 3 {4; } else {5; };
                a = if 2 == 3 {4; } elif 3 < 4 {1; } else {5; };";
            Assert.DoesNotThrow(() => CheckIt(s));
        }

        [Test]
        public void If_Cond_Not_Bool()
        {
            var s = @"mut a = if 2 + 3 {4; } else {5; };";
            Assert.Throws<TypeCheckException>(() => CheckIt(s));
        }

        [Test]
        public void If_Ret_Not_Same()
        {
            var s = @"mut a = if 2 + 3 {4; } else {true; };";
            Assert.Throws<TypeCheckException>(() => CheckIt(s));
        }

        [Test]
        public void If_Stmt()
        {
            var s = @"let a = if 2 > 3 {4; };";
            Assert.Throws<TypeCheckException>(() => CheckIt(s));
            s = "if 3 <= 4 {5;5;} elif 4 >= 3 {1;};";
            Assert.DoesNotThrow(() => CheckIt(s));
        }

        [Test]
        public void Func_Body_Ret_Not_Same()
        {
            var s = "func ab() -> Int {2;4; 5.5; }";
            Assert.Throws<TypeCheckException>(() => CheckIt(s));
        }

    }
}
