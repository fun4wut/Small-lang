namespace Small_lang.AST
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public class ForStmtAST : BaseAST
    {
        public ForStmtAST(Branch infLoop, BaseAST preRun, BaseAST postRun)
        {
            PreRun = preRun;
            PostRun = postRun;
            InfLoop = infLoop;
        }

        public BaseAST PreRun { get; }
        public BaseAST PostRun { get; }
        public Branch InfLoop { get; }
        public override ASTType NodeType { get; protected set; } = ASTType.For;
        protected internal override void Accept(ASTVisitor visitor) => visitor.VisitAST(this);
    }
}