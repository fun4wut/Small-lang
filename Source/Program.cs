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
            compiler.PreProcess(@"
if 3-2 == 1 then
    write 1;
else
    write 2;
end");
            compiler.Compile(Console.Out);
        }
    }
}
