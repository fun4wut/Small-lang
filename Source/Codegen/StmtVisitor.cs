using System;
using System.Collections.Generic;
using Small_lang.AST;

namespace Small_lang.Codegen
{
    partial class Codegenerator : ASTVisitor
    {
        private List<string> _genCode = new List<string>();
        protected internal override void VisitAST(ReadStmtAST node)
        {
            _genCode.Add($"in {node.VarType}");
        }

        protected internal override void VisitAST(WriteStmtAST node)
        {
            _genCode.Add($"out {node.Variable.RetType}");
        }
    }
}