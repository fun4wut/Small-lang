using System;
using System.IO;
using System.Runtime.InteropServices;
using Kumiko_lang.AST;
using Kumiko_lang.Codegen;
using LLVMSharp;

namespace Kumiko_lang
{
    class Program
    {
        private static string cwd = Directory.GetCurrentDirectory();
        private static void Main(string[] args)
        {
            var compiler = new Compiler();
            string s;
            string whole = "";
            whole = File.ReadAllText("../../../TextFile1.txt");
            //while ((s = Console.ReadLine()) != null)
            //{
            //    if (s == "") continue;
            //    whole += s;
            //}
            compiler.Compile(whole);
            compiler.Run();
        }
    }
}
