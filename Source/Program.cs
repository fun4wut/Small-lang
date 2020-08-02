using System;
using System.Runtime.InteropServices;
using Kumiko_lang.Codegen;
using LLVMSharp;

namespace Kumiko_lang
{
    class Program
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int Add(int a, int b);

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
                var stmt = LangParser.ParseSingle(s);
                visitor.Visit(stmt);

                Console.Write("Output: ");
                if (visitor.ResultStack.TryPop(out var v))
                {
                    LLVM.DumpValue(v);
                    Console.WriteLine();
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
