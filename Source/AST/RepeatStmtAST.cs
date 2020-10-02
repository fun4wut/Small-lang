using Small_lang.TypeCheck;
using System;
using System.Collections.Generic;
using System.Text;

namespace Small_lang.AST
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public sealed class RepeatStmtAST : BaseAST
    {
        public RepeatStmtAST(Branch infLoop)
        {
            InfLoop = infLoop;
        }
        public Branch InfLoop { get; }
        public override ASTType NodeType { get; protected set; } = ASTType.Repeat;

        protected internal override void Accept(ASTVisitor visitor) => visitor.VisitAST(this);
    }
}
