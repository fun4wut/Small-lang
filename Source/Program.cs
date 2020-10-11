using System;
using System.Diagnostics;
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
        if a == i
        then
            write a;
            break
        end
    until a % i == 0
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
            compiler.PreProcess(Prime);
            compiler.Compile(@"D:\VSWorkspace\Small-lang\PMachine\test.p");
            // using (var p = new Process
            // {
            //     StartInfo = new ProcessStartInfo("node", @"D:\VSWorkspace\Small-lang\PMachine\PMachine.js D:\VSWorkspace\Small-lang\Source\aa.p")
            // })
            // {
            //     p.StartInfo.UseShellExecute = false;
            //     p.Start();
            //     p.WaitForExit();
            // }
            
        }
    }
}
