using System;
using System.Collections.Generic;
using System.Text;

namespace Kumiko_lang.AST
{
    public abstract class ExprAST
    {
        public abstract ExprType NodeType { get; protected set; }

        protected internal abstract ExprAST? Accept(ExprVisitor visitor);
    }

    public static class ASTExtensions
    {
        public static void Compile(this List<ExprAST> exprASTs, ExprVisitor visitor) =>
             exprASTs.ForEach(expr => visitor.Visit(expr));

        public static void Compile(this ExprAST exprAST, ExprVisitor visitor) => visitor.Visit(exprAST);
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
