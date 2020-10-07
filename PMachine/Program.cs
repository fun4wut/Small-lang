using System;
using System.IO;
using System.Linq;

namespace PMachine
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = args.FirstOrDefault(s => s != "-v") ?? @"D:\VSWorkspace\Small-lang\PMachine\test.p";
            var machine = new Interpreter(path);
            var verbose = args.Any(s => s == "-v");
            machine.Run(verbose);
        }
    }
}