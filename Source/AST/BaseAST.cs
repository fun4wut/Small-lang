using System;
using System.Collections.Generic;
using System.Text;
using LLVMSharp;
namespace Kumiko_lang.AST
{
    public abstract class BaseAST
    {
        public abstract ASTType NodeType { get; protected set; }

        public virtual TypeKind RetType { get; set; } = TypeKind.Unit;

        public bool IsExpr => RetType != TypeKind.Unit;

        protected internal abstract BaseAST? Accept(ExprVisitor visitor);

        protected internal abstract void CheckWith(TypeCheker cheker);
    }


    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public struct TypedArg
    {
        public TypedArg(string name, TypeKind type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; }
        public TypeKind Type { get; }
    }
}
