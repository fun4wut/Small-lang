using System;
using System.Collections.Generic;
using System.Linq;
using Small_lang.AST;

namespace Small_lang.Codegen
{
    partial class Codegenerator : ASTVisitor
    {
        public List<string> GenCode { get; } = new List<string>();
        
        public override void Clear()
        {
            GenCode.Clear();
            _symTbl.Clear();
            _loops.Clear();
            _allocated = 0;
        }
        
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

        protected internal override void VisitAST(IfStmtAST node)
        {
            var elseLabel = Ins.CreateLabel();
            var endLabel = Ins.CreateLabel();
            Branch ifBranch = node.Branches.First();
            this.Visit(ifBranch.Cond);
            GenCode.Add(Ins.Fjp(elseLabel));
            this.Visit(ifBranch.Body);
            
            GenCode.Add(Ins.Ujp(endLabel));
            GenCode.Add(Ins.Label(elseLabel));
            
            this.Visit(node.ElseBranch?.Body);
            GenCode.Add(Ins.Label(endLabel));
        }

        protected internal override void VisitAST(ForStmtAST node)
        {
            this.Visit(node.PreRun); // init
            var (postLabel, endLabel) = OpenLoop();
            var beginLabel = Ins.CreateLabel();
            GenCode.Add(Ins.Label(beginLabel));
            this.Visit(node.InfLoop.Cond);
            // repeat's logic is opposite to for
            if (node.NodeType == ASTType.Repeat)
            {
                GenCode.Add(Ins.Not());
            }
            GenCode.Add(Ins.Fjp(endLabel));
            // body
            this.Visit(node.InfLoop.Body);
            // post run
            GenCode.Add(Ins.Label(postLabel));
            this.Visit(node.PostRun);
            // jump to cond
            GenCode.Add(Ins.Ujp(beginLabel));
            GenCode.Add(Ins.Label(endLabel));
            
            CloseLoop();
        }
        
        protected internal override void VisitAST(RepeatStmtAST node)
        {
            var (beginLabel, endLabel) = OpenLoop();
            GenCode.Add(Ins.Label(beginLabel));
            
            this.Visit(node.InfLoop.Body);
            
            // repeat's logic is opposite to for
            this.Visit(node.InfLoop.Cond);
            GenCode.Add(Ins.Not());
            
            GenCode.Add(Ins.Fjp(endLabel));
            GenCode.Add(Ins.Ujp(beginLabel));
            GenCode.Add(Ins.Label(endLabel));

            CloseLoop();
        }
    }
}