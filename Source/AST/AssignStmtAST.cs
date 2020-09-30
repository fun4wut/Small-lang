using Small_lang.TypeCheck;
using System;
using System.Collections.Generic;
using System.Text;

namespace Small_lang.AST
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public sealed class AssignStmtAST : BaseAST
    {
        public AssignStmtAST(string name, BaseAST value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public BaseAST Value { get; }
        public override ASTType NodeType { get; protected set; } = ASTType.Assign;

        protected internal override void Accept(ExprVisitor visitor) => visitor.VisitAST(this);

    }
}
