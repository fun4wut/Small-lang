﻿namespace Small_lang.AST
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public class ContinueStmtAST : BaseAST
    {
        public override ASTType NodeType { get; protected set; } = ASTType.Continue;
        protected internal override void Accept(ASTVisitor visitor) => visitor.VisitAST(this);
    }
}