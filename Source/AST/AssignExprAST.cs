using System;
using System.Collections.Generic;
using System.Text;

namespace Kumiko_lang.AST
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public sealed class AssignExprAST : ExprAST
    {
        public AssignExprAST(string name, ExprAST value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public ExprAST Value { get; }
        public override ExprType NodeType { get; protected set; } = ExprType.AssignExpr;

        protected internal override ExprAST? Accept(ExprVisitor visitor)
        {
            return base.Accept(visitor);
        }
    }
}
