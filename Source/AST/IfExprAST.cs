using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kumiko_lang.AST
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public sealed class IfExprAST : ExprAST
    {
        public IfExprAST(IEnumerable<Branch> branches)
        {
            Branches = branches.ToList();
        }

        public List<Branch> Branches { get; }

        public override ExprType NodeType { get; protected set; } = ExprType.IfExpr;

        protected internal override ExprAST? Accept(ExprVisitor visitor)
        {
            throw new NotImplementedException();
        }
    }

    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public class Branch
    {

        public ExprAST Cond;
        public List<ExprAST> Actions;

        public Branch(ExprAST cond, IEnumerable<ExprAST> actions)
        {
            Cond = cond;
            Actions = actions.ToList();
        }
    }

}
