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
        private List<BaseAST> _programAST = new List<BaseAST>();
        public void PreProcess(string s)
        {
            var prog = LangParser.ParseAll(s);
            _programAST = _checker.ReorderAndCheck(prog);
            _programAST.ForEach(Console.WriteLine);
        }

        public void Compile(string path)
        {
            _generator.Visit(_programAST);
            _generator.GenCode.Add(Ins.Hlt()); // halt the VM machine
            File.WriteAllLines(path, _generator.GenCode);
            //_generator.GenCode.ForEach(output.WriteLine);
        }
        
    }
}
