using Small_lang.TypeCheck;
using System;
using System.Collections.Generic;
using System.Text;

namespace Small_lang.AST
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public class WriteStmtAST : BaseAST
    {
        public WriteStmtAST(BaseAST value)
        {
            Value = value;
        }

        public BaseAST Value { get; }
        public override ASTType NodeType { get; protected set; } = ASTType.Write;

        protected internal override void Accept(ASTVisitor visitor) => visitor.VisitAST(this);

    }
}
