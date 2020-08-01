using System;
using Pidgin.Expression;
using System.Collections.Generic;
using System.Text;

namespace Kumiko_lang.AST
{
    [ToString]
    public sealed class BinaryExprAST : ExprAST
    {
        public BinaryExprAST(ExprType nodeType, ExprAST lhs, ExprAST rhs)
        {
            NodeType = nodeType;
            Lhs = lhs;
            Rhs = rhs;
        }

        public override ExprType NodeType { get; protected set; }
        public ExprAST Lhs { get; private set; }
        public ExprAST Rhs { get; private set; }

        protected internal override ExprAST? Accept(ExprVisitor visitor) => visitor.VisitBinaryExprAST(this);
    }
}
