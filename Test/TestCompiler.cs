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
            var ast = LangParser.ParseAll(s);
            ast.ForEach(expr => visitor.Visit(expr));
            Assert.AreEqual("i64 3", visitor.ResultStack.Pop().ToString());
        }

        [Test]
        public void Assignment()
        {
            var s = "let a = -5; a * 2 + 2;";
            var ast = LangParser.ParseAll(s);
            ast.ForEach(expr => visitor.Visit(expr));
            Assert.AreEqual("i64 -8", visitor.ResultStack.Pop().ToString());
        }
    }
}
