using System;
using System.IO;
using Kumiko_lang.AST;
using Kumiko_lang.Codegen;

namespace Kumiko_lang
{
    class Program
    {
        private static string cwd = Directory.GetCurrentDirectory();
        private static void Main(string[] args)
        {
            var compiler = new Compiler();
            string whole = File.ReadAllText(args[0]);
            compiler.Compile(whole);
            compiler.Dump2Stdout();
            compiler.Dump2File(args[1]);
            //compiler.Run();
        }
    }
}
