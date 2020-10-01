namespace Small_lang.AST
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public sealed class UnaryExprAST : BaseAST
    {
        public UnaryExprAST(ASTType nodeType, BaseAST hs)
        {
            NodeType = nodeType;
            Hs = hs;
        }

        public BaseAST Hs { get; }
        public override ASTType NodeType { get; protected set; }
        protected internal override void Accept(ASTVisitor visitor) => visitor.VisitAST(this);
    }
}