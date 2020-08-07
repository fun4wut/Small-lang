using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kumiko_lang.AST
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public sealed class ProtoStmtAST : BaseAST
    {
        public ProtoStmtAST(string name, IEnumerable<TypedArg> arguments, TypeKind ty)
        {
            Name = name;
            Arguments = arguments.ToList();
            RetType = ty;
        }

        public string Name { get; private set; }
        public List<TypedArg> Arguments { get; private set; }
        public TypeKind RetType { get; private set; }

        public override ASTType NodeType { get; protected set; } = ASTType.Prototype;

        protected internal override BaseAST? Accept(ExprVisitor visitor) => visitor.VisitAST(this);
    }
}
