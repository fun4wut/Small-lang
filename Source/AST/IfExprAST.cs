using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kumiko_lang.AST
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public class IfExprAST : BaseAST
    {
        public IfExprAST(IEnumerable<Branch> branches, ElseBranch? @else)
        {
            Branches = branches;
            ElseBranch = @else;
        }

        public IEnumerable<Branch> Branches { get; }

        public ElseBranch? ElseBranch;

        public override ASTType NodeType { get; protected set; } = ASTType.If;

        protected internal override BaseAST? Accept(ExprVisitor visitor) => visitor.VisitAST(this);
        protected internal override void CheckWith(TypeChecker checker) => checker.CheckAST(this);
    }

    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public class Branch
    {

        public BaseAST Cond;
        public BlockExprAST Body;

        public Branch(BaseAST cond, BlockExprAST body)
        {
            Cond = cond;
            Body = body;
        }
    }

    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public class ElseBranch
    {
        public BlockExprAST Body;

        public ElseBranch(BlockExprAST body)
        {
            Body = body;
        }
    }

}
