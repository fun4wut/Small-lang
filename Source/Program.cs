using System;
using System.IO;
using Small_lang.AST;
using Small_lang;

namespace Small_lang
{
    class Program
    {
        private const string Prime = @"
for a := 2; a <= 100; a := a + 1
begin
    for i := 2; i < a && a % i != 0; i := i+1
    begin
    end 
    if a == i
    then
        write a;
    end
end";

        private const string Gcd = @"
";
        private static void Main(string[] args)
        {
            var compiler = new Compiler();
            compiler.PreProcess(Prime);
            compiler.Compile("../../../aa.p");
        }
    }
}
