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
            Assert.AreEqual("i64 3", visitor.ResultStack.Pop().ToString());
        }

        [Test]
        public void Assignment()
        {
            var s = "let a = -5; a * 2 + 2;";
            LangParser.ParseAll(s).Compile(visitor);
            Assert.AreEqual("i64 -8", visitor.ResultStack.Pop().ToString());
        }

        [Test]
        public void DupDecl()
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
    }
}
