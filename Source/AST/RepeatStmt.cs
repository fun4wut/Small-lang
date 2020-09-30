using Small_lang.TypeCheck;
using System;
using System.Collections.Generic;
using System.Text;

namespace Small_lang.AST
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public class RepeatStmt : BaseAST
    {
        public RepeatStmt(Branch infLoop)
        {
            InfLoop = infLoop;
        }

        public Branch InfLoop { get; }
        public override ASTType NodeType { get; protected set; } = ASTType.Repeat;

        protected internal override void Accept(ExprVisitor visitor) => visitor.VisitAST(this);
    }
}
