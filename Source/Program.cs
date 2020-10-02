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
    i := 1;
    repeat
        i := i + 1;
    until i == a || a % i == 0
    if a == i
    then
        write a;
    end
end";

        private const string Gcd = @"
read a;
read b;
repeat
    tmp := b;
    b := a % b;
    a := tmp;
until b == 0
write a;";
        
        private static void Main(string[] args)
        {
            var compiler = new Compiler();
            compiler.PreProcess(Gcd);
            compiler.Compile("../../../aa.p");
        }
    }
}
