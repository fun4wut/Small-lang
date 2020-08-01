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

        public override bool Equals(object? obj)
        {
            return obj is BinaryExprAST aST &&
                   NodeType == aST.NodeType &&
                   NodeType == aST.NodeType &&
                   EqualityComparer<ExprAST>.Default.Equals(Lhs, aST.Lhs) &&
                   EqualityComparer<ExprAST>.Default.Equals(Rhs, aST.Rhs);
        }

        protected internal override ExprAST? Accept(ExprVisitor visitor) => visitor.VisitBinaryExprAST(this);
        
    }
}
