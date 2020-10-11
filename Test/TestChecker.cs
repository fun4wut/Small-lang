using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Small_lang.AST;
using Small_lang;
using Small_lang.TypeCheck;
namespace Test
{
    class TestChecker
    {
        private TypeChecker _checker = new TypeChecker();

        private void CheckIt(string s) => _checker.Visit(LangParser.ParseAll(s));

        [SetUp]
        public void Setup()
        {
            _checker = new TypeChecker();
        }
        

        [Test]
        public void Var_Var_Same_Name()
        {
            var s = "a := -5; a := 10";
            Assert.DoesNotThrow(() => CheckIt(s));
        }

        [Test]
        public void UndefinedVar()
        {
            var s = "a+1";
            Assert.Throws<TypeCheckException>(() => CheckIt(s));
        }

        [Test]
        public void Bool_Op()
        {
            var s = "4.0 == 2 + 3";
            Assert.DoesNotThrow(() => CheckIt(s));
            s = "4 < 2 == 4";
            Assert.Throws<TypeCheckException>(() => CheckIt(s));
            s = "1 < 2 && 12";
            Assert.Throws<TypeCheckException>(() => CheckIt(s));
        }

        [Test]
        public void If_Cond_Not_Bool()
        {
            var s = @"if 2 + 3 then 4 else 5 end";
            Assert.Throws<TypeCheckException>(() => CheckIt(s));
        }
        
        [Test]
        public void Read_Type_Mismatch()
        {
            var s = "a := 2.5; read a";
            Assert.Throws<TypeCheckException>(() => CheckIt(s));
        }

        [Test]
        public void Write_Undefined_Var()
        {
            var s = "write a";
            Assert.Throws<TypeCheckException>(() => CheckIt(s));
        }

        [Test]
        public void Repeat_Not_Bool()
        {
            var s = @"
repeat
    4
until 1";
            Assert.Throws<TypeCheckException>(() => CheckIt(s));
        }

    }
}
