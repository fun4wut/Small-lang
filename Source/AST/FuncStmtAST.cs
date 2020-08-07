using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kumiko_lang.AST
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public sealed class FuncStmtAST : BaseAST
    {
        public FuncStmtAST(ProtoStmtAST proto, IEnumerable<BaseAST> body)
        {
            Proto = proto;
            Body = body.ToList();
        }

        public ProtoStmtAST Proto { get; private set; }
        public List<BaseAST> Body { get; private set; }
        public override ASTType NodeType { get; protected set; } = ASTType.Function;

        protected internal override BaseAST? Accept(ExprVisitor visitor) => visitor.VisitAST(this);
    }
}
