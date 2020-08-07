using System;
using System.Collections.Generic;
using System.Text;
using LLVMSharp;
namespace Kumiko_lang.AST
{
    public abstract class BaseAST
    {
        public abstract ASTType NodeType { get; protected set; }

        protected internal abstract BaseAST? Accept(ExprVisitor visitor);
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
