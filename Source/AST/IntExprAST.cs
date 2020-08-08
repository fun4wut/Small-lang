using System;
using System.Collections.Generic;
using System.Text;
using Kumiko_lang.TypeCheck;
namespace Kumiko_lang.AST
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
        public override ASTType NodeType { get; protected set; } = ASTType.IntIdent;

        protected internal override BaseAST? Accept(ExprVisitor visitor) => visitor.VisitAST(this);
        protected internal override void CheckWith(TypeChecker checker) => checker.CheckAST(this);
    }
}
