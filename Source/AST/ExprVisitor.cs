using System;
using System.Collections.Generic;
using System.Text;

namespace Kumiko_lang.AST
{
    public abstract class ExprVisitor
    {
        protected ExprVisitor() { }

        public virtual BaseAST? Visit(BaseAST? node) => node?.Accept(this);

        protected internal virtual BaseAST VisitAST(BinaryExprAST node) => node;

        protected internal virtual BaseAST VisitAST(DeclStmtAST node) => node;

        protected internal virtual BaseAST VisitAST(FloatExprAST node) => node;

        protected internal virtual BaseAST VisitAST(IntExprAST node) => node;

        protected internal virtual BaseAST VisitAST(VariableExprAST node) => node;

        protected internal virtual BaseAST VisitAST(ProtoStmtAST node, bool combineUse = false) => node;

        protected internal virtual BaseAST VisitAST(FuncStmtAST node, bool isMain = false) => node;

        protected internal virtual BaseAST VisitAST(CallExprAST node) => node;

        protected internal virtual BaseAST VisitAST(AssignStmtAST node) => node;

        protected internal virtual BaseAST VisitAST(IfExprAST node) => node;


        protected internal virtual BaseAST VisitAST(BlockExprAST node) => node;

    }
}
