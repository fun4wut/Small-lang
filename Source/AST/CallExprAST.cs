using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kumiko_lang.AST
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public class CallExprAST : BaseAST, IExpr
    {
        public CallExprAST(string callee, IEnumerable<BaseAST> args)
        {
            this.Callee = callee;
            this.Arguments = args.ToList();
        }

        public string Callee { get; private set; }

        public List<BaseAST> Arguments { get; private set; }

        public override ASTType NodeType { get; protected set; } = ASTType.Call;

        protected internal override BaseAST Accept(ExprVisitor visitor) => visitor.VisitAST(this);
    }
}
