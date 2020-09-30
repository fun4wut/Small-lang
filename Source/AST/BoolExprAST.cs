using System;
using System.Collections.Generic;
using System.Text;
using Small_lang.TypeCheck;
namespace Small_lang.AST
{
    public sealed class BoolExprAST : BaseAST
    {
        public BoolExprAST(bool value)
        {
            Value = value;
        }

        public bool Value { get; }
        public override ASTType NodeType { get; protected set; } = ASTType.BoolLit;

        protected internal override void Accept(ExprVisitor visitor) => visitor.VisitAST(this);

    }
}
