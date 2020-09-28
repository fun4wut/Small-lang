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
        public override ASTType NodeType { get; protected set; } = ASTType.BoolIdent;

        protected internal override BaseAST? Accept(ExprVisitor visitor) => visitor.VisitAST(this);

        protected internal override void CheckWith(TypeChecker checker) => checker.CheckAST(this);
    }
}
