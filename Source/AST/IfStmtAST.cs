using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kumiko_lang.AST
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public class IfStmtAST : BaseAST
    {
        public IfStmtAST(IEnumerable<Branch> branches)
        {
            Branches = branches;
        }

        public IEnumerable<Branch> Branches { get; }

        public override ASTType NodeType { get; protected set; } = ASTType.If;

        protected internal override BaseAST? Accept(ExprVisitor visitor) => visitor.VisitAST(this);
    }

    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public class Branch
    {

        public BaseAST Cond;
        public List<BaseAST> Actions;

        public Branch(BaseAST cond, IEnumerable<BaseAST> actions)
        {
            Cond = cond;
            Actions = actions.ToList();
        }
    }

}
