using System;
using System.Collections.Generic;
using System.Text;
using LLVMSharp;
using Kumiko_lang.AST;

namespace Kumiko_lang.Codegen
{
    static class Utils
    {
        public static LLVMTypeRef ToLLVM(this TypeEnum ty) =>
            ty switch
            {
                TypeEnum.Int => LLVM.Int64Type(),
                TypeEnum.Float => LLVM.DoubleType(),
                TypeEnum.Bool => LLVM.Int1Type()
            };
    }

    public static class ASTExtensions
    {
        public static void Compile(this List<ExprAST> exprASTs, CodeGenVisitor visitor) => exprASTs.ForEach(e => visitor.Visit(e));

        public static void Compile(this ExprAST exprAST, ExprVisitor visitor) => visitor.Visit(exprAST);
    }
}
