using System;
using System.IO;
using Small_lang.AST;
using Small_lang.Codegen;

namespace Small_lang
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
