using System;
using System.Collections.Generic;
using System.Text;

namespace Kumiko_lang.AST
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public sealed class FloatExprAST : BaseAST, IExpr
    {
        public FloatExprAST(double value)
        {
            Value = value;
        }

        public double Value { get; }
        public override ASTType NodeType { get; protected set; } = ASTType.Float;
        protected internal override BaseAST? Accept(ExprVisitor visitor) => visitor.VisitAST(this);
    }
}
