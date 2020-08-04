using System;
using System.Collections.Generic;
using System.Text;

namespace Kumiko_lang.AST
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public sealed class FloatExprAST : ExprAST
    {
        public FloatExprAST(double value)
        {
            Value = value;
        }

        public double Value { get; }
        public override ExprType NodeType { get; protected set; } = ExprType.FloatExpr;
        protected internal override ExprAST? Accept(ExprVisitor visitor) => visitor.VisitAST(this);
    }
}
