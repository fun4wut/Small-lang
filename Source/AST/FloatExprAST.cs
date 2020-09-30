using System;
using System.Collections.Generic;
using System.Text;
using Small_lang.TypeCheck;
namespace Small_lang.AST
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public sealed class FloatExprAST : BaseAST
    {
        public FloatExprAST(double value)
        {
            Value = value;
        }

        public double Value { get; }
        public override ASTType NodeType { get; protected set; } = ASTType.FloatLit;
        protected internal override void Accept(ExprVisitor visitor) => visitor.VisitAST(this);
    }
}
