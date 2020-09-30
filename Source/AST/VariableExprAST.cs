using System;
using System.Collections.Generic;
using System.Text;
using Small_lang.TypeCheck;
namespace Small_lang.AST
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public sealed class VariableExprAST : BaseAST
    {
        public VariableExprAST(string name)
        {
            this.Name = name;
        }

        public string Name { get; }
        public override ASTType NodeType { get; protected set; } = ASTType.Variable;

        protected internal override void Accept(ASTVisitor visitor) => visitor.VisitAST(this);
    }
}
