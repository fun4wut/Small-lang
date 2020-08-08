using System;
using System.Collections.Generic;
using System.Text;

namespace Kumiko_lang.AST
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public sealed class IfExprAST : IfStmtAST, IExpr
    {
        public IfExprAST(IEnumerable<Branch> branches) : base(branches) { }

        public override ASTType NodeType { get; protected set; } = ASTType.If;

        protected internal override BaseAST? Accept(ExprVisitor visitor) => visitor.VisitAST(this);
    }
}
