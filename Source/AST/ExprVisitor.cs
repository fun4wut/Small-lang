using System;
using System.Collections.Generic;
using System.Text;

namespace Kumiko_lang.AST
{
    public abstract class ExprVisitor
    {
        protected ExprVisitor() { }

        public virtual ExprAST? Visit(ExprAST? node) => node?.Accept(this);

        protected internal virtual ExprAST VisitAST(BinaryExprAST node) => node;

        protected internal virtual ExprAST VisitAST(AssignExprAST node) => node;

        protected internal virtual ExprAST VisitAST(FloatExprAST node) => node;

        protected internal virtual ExprAST VisitAST(IntExprAST node) => node;

        protected internal virtual ExprAST VisitAST(VariableExprAST node) => node;

        protected internal virtual ExprAST VisitAST(ProtoExprAST node, bool combineUse = false) => node;

        protected internal virtual ExprAST VisitAST(FuncExprAST node, bool isMain = false) => node;

        protected internal virtual ExprAST VisitAST(CallExprAST node) => node;

    }
}
