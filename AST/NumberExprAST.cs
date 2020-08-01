using System;
using System.Collections.Generic;
using System.Text;

namespace Kumiko_lang.AST
{
    [ToString]
    public sealed class NumberExprAST : ExprAST
    {

        public NumberExprAST(int val)
        {
            this.Value = val;
        }

        public int Value { get; private set; }
        public override ExprType NodeType { get; protected set; } = ExprType.NumberExpr;

        protected internal override ExprAST? Accept(ExprVisitor visitor)
        {
            return base.Accept(visitor);
        }
    }
}
