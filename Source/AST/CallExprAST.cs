using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kumiko_lang.AST
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public class CallExprAST : ExprAST
    {
        public CallExprAST(string callee, IEnumerable<ExprAST> args)
        {
            this.Callee = callee;
            this.Arguments = args.ToList();
        }

        public string Callee { get; private set; }

        public List<ExprAST> Arguments { get; private set; }

        public override ExprType NodeType { get; protected set; } = ExprType.CallExpr;

        protected internal override ExprAST Accept(ExprVisitor visitor) => visitor.VisitAST(this);
    }
}
