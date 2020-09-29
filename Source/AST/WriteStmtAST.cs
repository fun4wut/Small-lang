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
        public WriteStmtAST(VariableExprAST variable)
        {
            Variable = variable;
        }

        public VariableExprAST Variable { get; set; }
        public override ASTType NodeType { get; protected set; } = ASTType.Write;

        protected internal override BaseAST? Accept(ExprVisitor visitor) => visitor.VisitAST(this);

        protected internal override void CheckWith(TypeChecker checker) => checker.CheckAST(this);
    }
}
