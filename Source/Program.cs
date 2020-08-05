using System;
using System.Runtime.InteropServices;
using Kumiko_lang.AST;
using Kumiko_lang.Codegen;
using LLVMSharp;

namespace Kumiko_lang
{
    class Program
    {
        private static void Main(string[] args)
        {
            // Make the module, which holds all the code.
            LLVMModuleRef module = LLVM.ModuleCreateWithName("my cool jit");
            LLVMBuilderRef builder = LLVM.CreateBuilder();

            LLVM.LinkInMCJIT();
            LLVM.InitializeX86TargetMC();
            LLVM.InitializeX86Target();
            LLVM.InitializeX86TargetInfo();
            LLVM.InitializeX86AsmParser();
            LLVM.InitializeX86AsmPrinter();
            
            var visitor = new CodeGenVisitor(module, builder);
            string s;
            string whole = "";
            while ((s = Console.ReadLine()) != null)
            {
                if (s == "") continue;
                whole += s;
                //LangParser.ParseSingle(s).Compile(visitor);

                //Console.Write("Output: ");
                //if (visitor.ResultStack.TryPop(out var v))
                //{
                //    Console.WriteLine(v.PrintValueToString());
                //}
                //else
                //{
                //    Console.WriteLine("void");
                //}
            }
            LangParser.ParseAll(whole).Compile(visitor);
            if (LLVM.CreateExecutionEngineForModule(out var engine, module, out var errorMessage).Value == 1)
            {
                Console.WriteLine(errorMessage);
                // LLVM.DisposeMessage(errorMessage);
                return;
            }
            if (visitor.MainFn != null)
            {
                LLVMValueRef main = (LLVMValueRef)visitor.MainFn;
                var res = LLVM.RunFunction(engine, main, new LLVMGenericValueRef[] { });
                var dotRes = main.TypeOf().GetReturnType().GetReturnType().TypeKind switch
                {
                    LLVMTypeKind.LLVMIntegerTypeKind => LLVM.GenericValueToInt(res, true),
                    LLVMTypeKind.LLVMDoubleTypeKind => LLVM.GenericValueToFloat(LLVM.DoubleType(), res),
                    _ => throw new NotImplementedException()
                };
                Console.WriteLine(dotRes);
            }

            LLVM.DumpModule(module);
        }
    }
}
