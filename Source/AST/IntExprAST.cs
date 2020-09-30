using System;
using System.Collections.Generic;
using System.Text;
using Small_lang.TypeCheck;
namespace Small_lang.AST
{
    [ToString]
    [Equals(DoNotAddEqualityOperators =true)]
    public sealed class IntExprAST : BaseAST
    {

        public IntExprAST(int val)
        {
            this.Value = val;
        }

        public int Value { get; private set; }
        public override ASTType NodeType { get; protected set; } = ASTType.IntLit;

        protected internal override void Accept(ExprVisitor visitor) => visitor.VisitAST(this);
    }
}
