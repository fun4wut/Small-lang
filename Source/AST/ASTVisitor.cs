using System;
using System.Collections.Generic;
using System.Text;

namespace Small_lang.AST
{
    public abstract class ASTVisitor
    {
        protected ASTVisitor() { }
        public abstract void Clear();
        public virtual void Visit(BaseAST? node) => node?.Accept(this);

        public void Visit(List<BaseAST> nodes) => nodes.ForEach(this.Visit);

        protected internal virtual void VisitAST(BinaryExprAST node) { }

        protected internal virtual void VisitAST(FloatExprAST node) { }

        protected internal virtual void VisitAST(IntExprAST node) { }

        protected internal virtual void VisitAST(BoolExprAST node) { }

        protected internal virtual void VisitAST(VariableExprAST node) { }

        protected internal virtual void VisitAST(AssignStmtAST node) { }

        protected internal virtual void VisitAST(IfStmtAST node) { }

        protected internal virtual void VisitAST(BlockExprAST node) { }

        protected internal virtual void VisitAST(ReadStmtAST node) { }

        protected internal virtual void VisitAST(WriteStmtAST node) { }

        protected internal virtual void VisitAST(RepeatStmtAST node) { }
        
        protected internal virtual void VisitAST(ForStmtAST node) { }
        
        protected internal virtual void VisitAST(UnaryExprAST node) { }
        
        protected internal virtual void VisitAST(BreakStmtAST node) { }

        protected internal virtual void VisitAST(ContinueStmtAST node) { }
    }
}
