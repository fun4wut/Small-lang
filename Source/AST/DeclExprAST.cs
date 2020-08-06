using System;
using System.Collections.Generic;
using System.Text;

namespace Kumiko_lang.AST
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public sealed class DeclExprAST : ExprAST
    {
        public DeclExprAST(ExprType ty, string name, ExprAST value)
        {
            NodeType = ty;
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public ExprAST Value { get; }
        public override ExprType NodeType { get; protected set; }

        protected internal override ExprAST? Accept(ExprVisitor visitor) => visitor.VisitAST(this);
    }
}
