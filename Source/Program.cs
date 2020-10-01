using System;
using System.IO;
using Small_lang.AST;
using Small_lang;

namespace Small_lang
{
    class Program
    {
        private static void Main(string[] args)
        {
            var compiler = new Compiler();
            compiler.PreProcess("read a; b := 3; c := -a + b; write b + c;");
            compiler.Compile(Console.Out);
        }
    }
}
