using System;
using System.Collections.Generic;
using System.Text;

namespace Kumiko_lang.AST
{
    public abstract class ExprAST
    {
        public abstract ExprType NodeType { get; protected set; }

        protected internal abstract ExprAST? Accept(ExprVisitor visitor);
    }
}
