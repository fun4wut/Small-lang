using System;
using System.Collections.Generic;
using System.IO;
using Small_lang.AST;
using Small_lang.Codegen;
using Small_lang.TypeCheck;

namespace Small_lang
{
    public class Compiler
    {
        private TypeChecker _checker = new TypeChecker();
        private Codegenerator _generator = new Codegenerator();
        public List<BaseAST> ProgramAST { get; private set; } = new List<BaseAST>();
        public void PreProcess(string s)
        {
            var prog = LangParser.ParseAll(s);
            _checker.Visit(prog);
            prog.ForEach(Console.WriteLine);
            ProgramAST = prog;
        }

        public void Compile(TextWriter output)
        {
            _generator.Visit(ProgramAST);
            _generator.GenCode.Add(Ins.Hlt()); // halt the VM machine
            _generator.GenCode.ForEach(output.WriteLine);
        }
        
    }
}
