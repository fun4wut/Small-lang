namespace Small_lang.AST
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public class BreakStmtAST : BaseAST
    {
        public override ASTType NodeType { get; protected set; } = ASTType.Break;
        protected internal override void Accept(ASTVisitor visitor) => visitor.VisitAST(this);
    }
}