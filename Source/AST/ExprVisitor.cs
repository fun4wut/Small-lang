using System;
using System.Collections.Generic;
using System.Text;

namespace Small_lang.AST
{
    public abstract class ExprVisitor
    {
        protected ExprVisitor() { }

        public virtual void Visit(BaseAST? node) => node?.Accept(this);

        public void Visit(List<BaseAST> nodes) => nodes.ForEach(this.Visit);

        protected internal abstract void VisitAST(BinaryExprAST node);

        protected internal abstract void VisitAST(FloatExprAST node);

        protected internal abstract void VisitAST(IntExprAST node);

        protected internal abstract void VisitAST(BoolExprAST node);

        protected internal abstract void VisitAST(VariableExprAST node);

        protected internal abstract void VisitAST(ProtoStmtAST node);

        protected internal abstract void VisitAST(FuncStmtAST node);

        protected internal abstract void VisitAST(CallExprAST node);

        protected internal abstract void VisitAST(AssignStmtAST node);

        protected internal abstract void VisitAST(IfStmtAST node);

        protected internal abstract void VisitAST(BlockExprAST node);

        protected internal abstract void VisitAST(ReadStmtAST node);

        protected internal abstract void VisitAST(WriteStmtAST node);

        protected internal abstract void VisitAST(RepeatStmt node);

    }
}
