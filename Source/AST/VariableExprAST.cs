using System;
using System.Collections.Generic;
using System.Text;

namespace Kumiko_lang.AST
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public sealed class VariableExprAST : BaseAST, IExpr
    {
        public VariableExprAST(string name)
        {
            this.Name = name;
        }

        public string Name { get; }
        public override ASTType NodeType { get; protected set; } = ASTType.Variable;

        protected internal override BaseAST? Accept(ExprVisitor visitor) => visitor.VisitAST(this);
    }
}
