using System;
using System.Collections.Generic;
using System.Text;

namespace Kumiko_lang.AST
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public sealed class VariableExprAST : ExprAST
    {
        public VariableExprAST(string name)
        {
            this.Name = name;
        }

        public string Name { get; }
        public override ExprType NodeType { get; protected set; } = ExprType.VariableExpr;

        protected internal override ExprAST? Accept(ExprVisitor visitor) => visitor.VisitAST(this);
    }
}
