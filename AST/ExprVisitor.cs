using System;
using System.Collections.Generic;
using System.Text;

namespace Kumiko_lang.AST
{
    public abstract class ExprVisitor
    {
        protected ExprVisitor() { }

        public virtual ExprAST? Visit(ExprAST? node) => node?.Accept(this);

        protected internal virtual ExprAST? VisitExtension(ExprAST node) => node.VisitChildren(this);

        protected internal virtual ExprAST VisitBinaryExprAST(BinaryExprAST node)
        {
            this.Visit(node.Lhs);
            this.Visit(node.Rhs);

            return node;
        }
    }
}
