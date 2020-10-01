using System;
using System.Collections.Generic;
using Small_lang.AST;

namespace Small_lang.Codegen
{
    partial class Codegenerator : ASTVisitor
    {
        public List<string> GenCode { get; } = new List<string>();
        protected internal override void VisitAST(ReadStmtAST node)
        {
            RegisterId(node.Name, node.VarType);
            GenCode.Add(Ins.In(node.VarType));
        }

        protected internal override void VisitAST(WriteStmtAST node)
        {
            this.Visit(node.Value);
            GenCode.Add(Ins.Out(node.Value.RetType));
        }

        protected internal override void VisitAST(AssignStmtAST node)
        {
            if (!HasId(node.Name))
            {
                RegisterId(node.Name, node.Value.RetType);
            }
            var (ty, addr) = GetId(node.Name);
            this.Visit(node.Value);
            GenCode.Add(Ins.Dpl(ty)); // duplicate the stack top first
            GenCode.Add(Ins.Str(ty, 0, addr)); // then move to the specified addr, sp--
        }


    }
}