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
            var visitor = new CodeGenVisitor(module, builder);
            string s;
            while ((s = Console.ReadLine()) != null)
            {
                if (s == "") continue;
                LangParser.ParseSingle(s).Compile(visitor);

                Console.Write("Output: ");
                if (visitor.ResultStack.TryPop(out var v))
                {
                    Console.WriteLine(v.PrintValueToString());
                }
                else
                {
                    Console.WriteLine("void");
                }
            }
            LLVM.DumpModule(module);
        }
    }
}
