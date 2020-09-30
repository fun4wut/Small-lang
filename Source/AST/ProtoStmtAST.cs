using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Small_lang.TypeCheck;
namespace Small_lang.AST
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public sealed class ProtoStmtAST : BaseAST
    {
        public ProtoStmtAST(string name, IEnumerable<TypedArg> arguments, TypeKind ty)
        {
            Name = name;
            Arguments = arguments.ToList();
            FnRet = ty;
        }

        public string Name { get; private set; }
        public List<TypedArg> Arguments { get; private set; }
        public TypeKind FnRet { get; private set; }

        public override ASTType NodeType { get; protected set; } = ASTType.Prototype;

        protected internal override void Accept(ExprVisitor visitor) => visitor.VisitAST(this);
    }
}
