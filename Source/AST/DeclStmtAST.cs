using System;
using System.Collections.Generic;
using System.Text;

namespace Kumiko_lang.AST
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public sealed class DeclStmtAST : BaseAST
    {
        public DeclStmtAST(ASTType ty, string name, BaseAST value)
        {
            NodeType = ty;
            Name = name;
            Value = value;
        }


        public string Name { get; }
        public BaseAST Value { get; }
        public override ASTType NodeType { get; protected set; }

        protected internal override BaseAST? Accept(ExprVisitor visitor) => visitor.VisitAST(this);
        protected internal override void CheckWith(TypeCheker cheker) { }
    }
}
