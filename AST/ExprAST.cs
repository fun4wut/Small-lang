using System;
using System.Collections.Generic;
using System.Text;

namespace Kumiko_lang.AST
{
    public abstract class ExprAST
    {
        public abstract ExprType NodeType { get; protected set; }

        protected internal virtual ExprAST? VisitChildren(ExprVisitor visitor) => visitor.Visit(this);

        protected internal virtual ExprAST? Accept(ExprVisitor visitor) => visitor.VisitExtension(this);
    }
}
