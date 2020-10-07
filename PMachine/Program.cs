using System;
using System.IO;

namespace PMachine
{
    class Program
    {
        static void Main(string[] args)
        {
            var machine = new Interpreter(@"D:\VSWorkspace\Small-lang\PMachine\test.p");
            machine.Run(false);
        }
    }
}