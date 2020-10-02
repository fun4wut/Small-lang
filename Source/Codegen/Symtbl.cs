using System.Collections.Generic;
using Small_lang.AST;

namespace Small_lang.Codegen
{
    partial class Codegenerator : ASTVisitor
    {
        private Dictionary<string, (TypeKind, int)> _symTbl = new Dictionary<string, (TypeKind, int)>();
        private int _allocated = 0;
        private Stack<(string, string)> _loops = new Stack<(string, string)>();
        private void RegisterId(string name, TypeKind ty)
        {
            _symTbl[name] = (ty, _allocated);
            _allocated++;
        }

        private (TypeKind, int) GetId(string name)
        {
            return _symTbl[name];
        }

        private bool HasId(string name) => _symTbl.ContainsKey(name);

        public (string, string) OpenLoop()
        {
            _loops.Push((Ins.CreateLabel(), Ins.CreateLabel()));
            return _loops.Peek();
        }
        public string LoopStart => _loops.Peek().Item1;
        public string LoopEnd => _loops.Peek().Item2;
    }
}