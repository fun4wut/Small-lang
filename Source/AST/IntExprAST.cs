using System;
using System.Collections.Generic;
using System.Text;

namespace Kumiko_lang.AST
{
    [ToString]
    [Equals(DoNotAddEqualityOperators =true)]
    public sealed class IntExprAST : ExprAST
    {

        public IntExprAST(int val)
        {
            this.Value = val;
        }

        public int Value { get; private set; }
        public override ExprType NodeType { get; protected set; } = ExprType.IntExpr;

        protected internal override ExprAST? Accept(ExprVisitor visitor) => visitor.VisitAST(this);
    }
}
