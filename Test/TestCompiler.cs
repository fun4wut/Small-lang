using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Kumiko_lang.Codegen;
using Kumiko_lang.AST;
using LLVMSharp;
using Kumiko_lang;

namespace Test
{
    class TestCompiler
    {
        LLVMModuleRef module;
        LLVMBuilderRef builder;
        CodeGenVisitor visitor;

        [SetUp]
        public void Setup()
        {
            module = LLVM.ModuleCreateWithName("Test JIT");
            builder = LLVM.CreateBuilder();
            visitor = new CodeGenVisitor(module, builder);
        }

        [Test]
        public void Arithmetic()
        {
            var s = "1+2;";
            LangParser.ParseAll(s).Compile(visitor);
            Assert.AreEqual("i64 3", visitor.PrintTop());
        }

        [Test]
        public void Assignment()
        {
            var s = "let a = -5; a * 2 + 2;";
            LangParser.ParseAll(s).Compile(visitor);
            Assert.AreEqual("i64 -8", visitor.PrintTop());
        }

        [Test]
        public void Var_Var_Same_Name()
        {
            var s = "let a = -5; let a = 10;";
            Assert.Throws<DupDeclException>(() => LangParser.ParseAll(s).Compile(visitor));
        }

        [Test]
        public void UndefinedVar()
        {
            var s = "a+1;";
            Assert.Throws<UndefinedVarException>(() => LangParser.ParseAll(s).Compile(visitor));
        }

        [Test]
        public void Proto()
        {
            var s = "func ab(a: Int, b: Float) -> Float;";
            LangParser.ParseAll(s).Compile(visitor);
            Assert.AreEqual("declare double @ab(i64, double)", visitor.PrintTop().Trim());
        }

        [Test]
        public void Proto_NoParam()
        {
            var s = "func ab() -> Int;";
            LangParser.ParseAll(s).Compile(visitor);
            Assert.AreEqual("declare i64 @ab()", visitor.PrintTop());
        }

        [Test]
        public void Func()
        {
            var s = "func ab(a: Int) -> Int {let b = a*10; b-a;};";
            LangParser.ParseAll(s).Compile(visitor);
            Assert.AreEqual(@"define i64 @ab(i64 %a) {
entry:
  %b = mul i64 %a, 10
  %subtmp = sub i64 %b, %a
  ret i64 %subtmp
}", visitor.PrintTop());
        }

        [Test]
        public void Var_Func_Same_Name()
        {
            var s = "func ab() -> Int; let ab = 1;";
            Assert.Throws<DupDeclException>(() => LangParser.ParseAll(s).Compile(visitor));
            s = "let ab = 1;func ab() -> Int;";
            Assert.Throws<DupDeclException>(() => LangParser.ParseAll(s).Compile(visitor));
        }

        [Test]
        public void Func_Func_Same_Name()
        {
            var s = "func ab() -> Int; func ab() -> Float;";
            Assert.Throws<DupDeclException>(() => LangParser.ParseAll(s).Compile(visitor));
            s = "func ab() -> Int; func ab() -> Int {4;};";
            Assert.DoesNotThrow(() => LangParser.ParseAll(s).Compile(visitor));
        }
    }
}
