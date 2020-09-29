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

        public Branch InfLoop { get; set; }
        public override ASTType NodeType { get; protected set; } = ASTType.Repeat;

        protected internal override BaseAST? Accept(ExprVisitor visitor) => visitor.VisitAST(this);

        protected internal override void CheckWith(TypeChecker checker) => checker.CheckAST(this);
    }
}
