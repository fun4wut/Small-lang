using System;
using System.Collections.Generic;
using System.Text;
using LLVMSharp;
using Kumiko_lang;
using Kumiko_lang.AST;
using System.Linq;

namespace Kumiko_lang.Codegen
{
    public class Compiler
    {
        LLVMModuleRef module = LLVM.ModuleCreateWithName("my cool jit");
        LLVMBuilderRef builder = LLVM.CreateBuilder();
        CodeGenVisitor visitor;
        LLVMExecutionEngineRef engine;
        public Compiler()
        {
            this.visitor = new CodeGenVisitor(this.module, this.builder);
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
            // reorder the AST
            exprs.Sort((e1, e2) => e1.NodeType.ASTValue() - e2.NodeType.ASTValue());
            var funs = exprs.TakeWhile(e => e.NodeType.ASTValue() < 0);
            var main = exprs.SkipWhile(e => e.NodeType.ASTValue() < 0);
            foreach (var expr in funs)
            {
                visitor.Visit(expr);
            }
            visitor.InsertMain(main);
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
