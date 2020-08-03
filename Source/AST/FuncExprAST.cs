using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kumiko_lang.AST
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public sealed class FuncExprAST : ExprAST
    {
        public FuncExprAST(ProtoExprAST proto, IEnumerable<ExprAST> body)
        {
            Proto = proto;
            Body = body.ToList();
        }

        public ProtoExprAST Proto { get; private set; }
        public List<ExprAST> Body { get; private set; }
        public override ExprType NodeType { get; protected set; } = ExprType.FunctionExpr;

        protected internal override ExprAST? Accept(ExprVisitor visitor) => visitor.VisitAST(this);
    }
}
