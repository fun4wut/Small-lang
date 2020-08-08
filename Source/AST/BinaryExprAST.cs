using System;
using Pidgin.Expression;
using System.Collections.Generic;
using System.Text;

namespace Kumiko_lang.AST
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public sealed class BinaryExprAST : BaseAST
    {
        public BinaryExprAST(ASTType nodeType, BaseAST lhs, BaseAST rhs)
        {
            NodeType = nodeType;
            Lhs = lhs;
            Rhs = rhs;
        }

        public override ASTType NodeType { get; protected set; }
        public BaseAST Lhs { get; private set; }
        public BaseAST Rhs { get; private set; }

        protected internal override BaseAST? Accept(ExprVisitor visitor) => visitor.VisitAST(this);
        protected internal override void CheckWith(TypeChecker checker) => checker.CheckAST(this);
    }
}
