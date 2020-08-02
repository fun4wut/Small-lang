using System;
using System.Collections.Generic;
using System.Text;

namespace Kumiko_lang.AST
{
    public abstract class ExprVisitor
    {
        protected ExprVisitor() { }

        public virtual ExprAST? Visit(ExprAST? node) => node?.Accept(this);

        protected internal abstract ExprAST VisitAST(BinaryExprAST node);

        protected internal abstract ExprAST VisitAST(AssignExprAST node);

        protected internal abstract ExprAST VisitAST(FloatExprAST node);

        protected internal abstract ExprAST VisitAST(IntExprAST node);

        protected internal abstract ExprAST VisitAST(VariableExprAST node);

    }
}
