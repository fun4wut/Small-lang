using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kumiko_lang.AST
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public sealed class ProtoExprAST : ExprAST
    {
        public ProtoExprAST(string name, IEnumerable<TypedArg> arguments, TypeEnum ty)
        {
            Name = name;
            Arguments = arguments.ToList();
            RetType = ty;
        }

        public string Name { get; private set; }
        public List<TypedArg> Arguments { get; private set; }
        public TypeEnum RetType { get; private set; }

        public override ExprType NodeType { get; protected set; } = ExprType.PrototypeExpr;

        protected internal override ExprAST? Accept(ExprVisitor visitor) => visitor.VisitAST(this);
    }
}
