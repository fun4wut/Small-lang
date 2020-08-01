using System;
using System.Collections.Generic;
using System.Text;

namespace Kumiko_lang.AST
{
    [ToString]
    public sealed class VariableExprAST : ExprAST
    {
        public VariableExprAST(string name)
        {
            this.Name = name;
        }

        public string Name { get; }
        public override ExprType NodeType { get; protected set; } = ExprType.VariableExpr;

        public override bool Equals(object? obj)
        {
            return obj is VariableExprAST aST &&
                   NodeType == aST.NodeType &&
                   Name == aST.Name &&
                   NodeType == aST.NodeType;
        }

        protected internal override ExprAST? Accept(ExprVisitor visitor)
        {
            return base.Accept(visitor);
        }
    }
}
