using System;
using System.Collections.Generic;
using System.Text;
using LLVMSharp;
using Kumiko_lang;
using Kumiko_lang.AST;
using System.Linq;
using Kumiko_lang.TypeCheck;
namespace Kumiko_lang.Codegen
{
    public class Compiler
    {
        LLVMModuleRef module = LLVM.ModuleCreateWithName("my cool jit");
        LLVMBuilderRef builder = LLVM.CreateBuilder();
        TypeChecker checker = new TypeChecker();
        CodeGenVisitor visitor;
        LLVMExecutionEngineRef engine;
        public Compiler()
        {
            this.visitor = new CodeGenVisitor(this.module, this.builder, this.checker);
            LLVM.LinkInMCJIT();
            LLVM.InitializeX86TargetMC();
            LLVM.InitializeX86Target();
            LLVM.InitializeX86TargetInfo();
            LLVM.InitializeX86AsmParser();
            LLVM.InitializeX86AsmPrinter();
            if (LLVM.CreateExecutionEngineForModule(out engine, module, out var errorMessage).Value == 1)
            {
                Console.WriteLine(errorMessage);
            }
        }

        public void Compile(string s)
        {
            var exprs = LangParser.ParseAll(s);
            var program = checker.ReorderAndCheck(exprs);
            visitor.Visit(program);
        }

        public void Run()
        {
            var main = visitor.MainFn;
            var res = LLVM.RunFunction(engine, main, new LLVMGenericValueRef[] { });
            var dotRes = main.TypeOf().GetReturnType().GetReturnType().TypeKind switch
            {
                LLVMTypeKind.LLVMIntegerTypeKind => LLVM.GenericValueToInt(res, true),
                LLVMTypeKind.LLVMDoubleTypeKind => LLVM.GenericValueToFloat(LLVM.DoubleType(), res),
                _ => throw new NotImplementedException()
            };
            Console.WriteLine(dotRes);
        }

        public void Dump2File(string file) => LLVM.PrintModuleToFile(this.module, file, out _);

        public void Dump2Stdout() => LLVM.DumpModule(this.module);

    }
}
