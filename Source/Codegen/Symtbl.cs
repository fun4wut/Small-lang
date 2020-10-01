using System.Collections.Generic;
using Small_lang.AST;

namespace Small_lang.Codegen
{
    partial class Codegenerator : ASTVisitor
    {
        private Dictionary<string, (TypeKind, int)> _symTbl = new Dictionary<string, (TypeKind, int)>();
        private int _allocated = 0;
        
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
    }
}