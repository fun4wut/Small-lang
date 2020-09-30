using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Small_lang.TypeCheck;

namespace Small_lang.AST
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public class CallExprAST : BaseAST
    {
        public CallExprAST(string callee, IEnumerable<BaseAST> args)
        {
            this.Callee = callee;
            this.Arguments = args.ToList();
        }

        public string Callee { get; }

        public List<BaseAST> Arguments { get; }

        public override ASTType NodeType { get; protected set; } = ASTType.Call;

        protected internal override void Accept(ExprVisitor visitor) => visitor.VisitAST(this);
    }
}
