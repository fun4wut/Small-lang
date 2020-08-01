using System;
using System.Linq;

namespace Kumiko_lang
{
    class Program
    {
        static void Main(string[] args)
        {
            var s = "ab*(1+2);1+33;";
            LangParser.ParseOrThrow(s).ToList().ForEach(Console.WriteLine);
        }
    }
}
