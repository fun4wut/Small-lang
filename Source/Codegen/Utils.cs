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
                TypeEnum.Bool => LLVM.Int1Type(),
                TypeEnum.Unit => LLVM.VoidType(),
                _ => throw new NotImplementedException()
            };
    }

    public static class ASTExtensions
    {
        public static void Compile(this List<ExprAST> exprASTs, CodeGenVisitor visitor) => 
            exprASTs.ForEach(e => visitor.Visit(e));

        public static void Compile(this ExprAST exprAST, ExprVisitor visitor) => visitor.Visit(exprAST);

        public static int ASTValue(this ExprType ty) => ty switch
        {
            ExprType.PrototypeExpr => -2,
            ExprType.FunctionExpr => -1,
            _ => 0
        };
    }

    static class LLVMExt
    {
        public static bool IsPtr(this LLVMValueRef val) => val.TypeOf().TypeKind == LLVMTypeKind.LLVMPointerTypeKind;

        public static bool isInt(this LLVMValueRef val) => val.TypeOf().TypeKind == LLVMTypeKind.LLVMIntegerTypeKind;

        public static bool isBool(this LLVMValueRef val) => val.isInt() && val.TypeOf().GetIntTypeWidth() == 1;

        public static bool isFloat(this LLVMValueRef val) => val.TypeOf().TypeKind == LLVMTypeKind.LLVMDoubleTypeKind;

        private static void AutoConvertType(this LLVMBuilderRef builder, ref LLVMValueRef a, ref LLVMValueRef b)
        {
            if (a.isBool() || b.isBool())
            {
                throw new Exception("bool can't perform bin op");
            }
            if (a.TypeOf().TypeKind == b.TypeOf().TypeKind)
            {
                return;
            }
            if (a.isInt())
            {
                a = LLVM.BuildSIToFP(builder, a, LLVM.DoubleType(), "convtmp");
            }
            else
            {
                b = LLVM.BuildSIToFP(builder, b, LLVM.DoubleType(), "convtmp");
            }
        }

        public static LLVMValueRef
            DoBinaryOps(this LLVMBuilderRef builder, ExprType ty, LLVMValueRef l, LLVMValueRef r)
        {
            builder.AutoConvertType(ref l, ref r);
            return l.TypeOf().TypeKind switch
            {
                LLVMTypeKind.LLVMIntegerTypeKind => ty switch
                {
                    ExprType.AddExpr => LLVM.BuildAdd(builder, l, r, "addtmp"),
                    ExprType.SubtractExpr => LLVM.BuildSub(builder, l, r, "subtmp"),
                    ExprType.MultiplyExpr => LLVM.BuildMul(builder, l, r, "multmp"),
                    ExprType.DivideExpr => LLVM.BuildSDiv(builder, l, r, "divtmp"),
                    ExprType.EqualExpr => LLVM.BuildICmp(builder, LLVMIntPredicate.LLVMIntEQ, l, r, "eqtmp"),
                    ExprType.LessThanExpr => LLVM.BuildICmp(builder, LLVMIntPredicate.LLVMIntSLT, l, r, "lttmp"),
                    ExprType.LessEqualExpr => LLVM.BuildICmp(builder, LLVMIntPredicate.LLVMIntSLE, l, r, "letmp"),
                    ExprType.GreaterThanExpr => LLVM.BuildICmp(builder, LLVMIntPredicate.LLVMIntSGT, l, r, "gttmp"),
                    ExprType.GreatEqualExpr => LLVM.BuildICmp(builder, LLVMIntPredicate.LLVMIntSGE, l, r, "getmp"),
                    _ => throw new NotImplementedException()
                },
                LLVMTypeKind.LLVMDoubleTypeKind => ty switch
                {
                    ExprType.AddExpr => LLVM.BuildFAdd(builder, l, r, "addtmp"),
                    ExprType.SubtractExpr => LLVM.BuildFSub(builder, l, r, "subtmp"),
                    ExprType.MultiplyExpr => LLVM.BuildFMul(builder, l, r, "multmp"),
                    ExprType.DivideExpr => LLVM.BuildFDiv(builder, l, r, "divtmp"),
                    ExprType.EqualExpr => LLVM.BuildFCmp(builder, LLVMRealPredicate.LLVMRealUEQ, l, r, "eqtmp"),
                    ExprType.LessThanExpr => LLVM.BuildFCmp(builder, LLVMRealPredicate.LLVMRealULT, l, r, "lttmp"),
                    ExprType.LessEqualExpr => LLVM.BuildFCmp(builder, LLVMRealPredicate.LLVMRealULE, l, r, "letmp"),
                    ExprType.GreaterThanExpr => LLVM.BuildFCmp(builder, LLVMRealPredicate.LLVMRealUGT, l, r, "gttmp"),
                    ExprType.GreatEqualExpr => LLVM.BuildFCmp(builder, LLVMRealPredicate.LLVMRealUGE, l, r, "getmp"),
                    _ => throw new NotImplementedException()
                },
            };

        }
    }
}
