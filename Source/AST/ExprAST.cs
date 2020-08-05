using System;
using System.Collections.Generic;
using System.Text;
using LLVMSharp;
namespace Kumiko_lang.AST
{
    public abstract class ExprAST
    {
        public abstract ExprType NodeType { get; protected set; }

        protected internal abstract ExprAST? Accept(ExprVisitor visitor);
    }



    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public struct TypedArg
    {
        public TypedArg(string name, TypeEnum type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; }
        public TypeEnum Type { get; }
    }
}
