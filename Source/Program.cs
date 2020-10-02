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
for a := 1; a < 10; a := a + 1
begin
    write a;
end");
            compiler.Compile(Console.Out);
        }
    }
}
