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

        public override bool Equals(object? obj)
        {
            return obj is NumberExprAST aST &&
                   NodeType == aST.NodeType &&
                   Value == aST.Value &&
                   NodeType == aST.NodeType;
        }

        protected internal override ExprAST? Accept(ExprVisitor visitor)
        {
            return base.Accept(visitor);
        }
    }
}
