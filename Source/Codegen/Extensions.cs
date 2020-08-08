using System;
using System.Collections.Generic;
using System.Text;
using LLVMSharp;
using Kumiko_lang.AST;

namespace Kumiko_lang.Codegen
{
    static class LLVMExt
    {
        public static bool IsPtr(this LLVMValueRef val) => val.TypeOf().TypeKind == LLVMTypeKind.LLVMPointerTypeKind;

        public static bool isInt(this LLVMValueRef val) => val.TypeOf().TypeKind == LLVMTypeKind.LLVMIntegerTypeKind;

        public static bool isBool(this LLVMValueRef val) => val.isInt() && val.TypeOf().GetIntTypeWidth() == 1;

        public static bool isFloat(this LLVMValueRef val) => val.TypeOf().TypeKind == LLVMTypeKind.LLVMDoubleTypeKind;

        private static void AutoConvertType(this LLVMBuilderRef builder, ref LLVMValueRef a, ref LLVMValueRef b, ASTType op)
        {
            if (op != ASTType.Equal && (a.isBool() || b.isBool()))
            {
                throw new Exception("bool can't perform bin op, except == ");
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
            DoBinaryOps(this LLVMBuilderRef builder, ASTType op, LLVMValueRef l, LLVMValueRef r)
        {
            builder.AutoConvertType(ref l, ref r, op);
            return l.TypeOf().TypeKind switch
            {
                LLVMTypeKind.LLVMIntegerTypeKind => op switch // bool is also integer in llvm
                {
                    ASTType.Add => LLVM.BuildAdd(builder, l, r, "addtmp"),
                    ASTType.Subtract => LLVM.BuildSub(builder, l, r, "subtmp"),
                    ASTType.Multiply => LLVM.BuildMul(builder, l, r, "multmp"),
                    ASTType.Divide => LLVM.BuildSDiv(builder, l, r, "divtmp"),
                    ASTType.Equal => LLVM.BuildICmp(builder, LLVMIntPredicate.LLVMIntEQ, l, r, "eqtmp"),
                    ASTType.LessThan => LLVM.BuildICmp(builder, LLVMIntPredicate.LLVMIntSLT, l, r, "lttmp"),
                    ASTType.LessEqual => LLVM.BuildICmp(builder, LLVMIntPredicate.LLVMIntSLE, l, r, "letmp"),
                    ASTType.GreaterThan => LLVM.BuildICmp(builder, LLVMIntPredicate.LLVMIntSGT, l, r, "gttmp"),
                    ASTType.GreaterEqual => LLVM.BuildICmp(builder, LLVMIntPredicate.LLVMIntSGE, l, r, "getmp"),
                    _ => throw new NotImplementedException()
                },
                LLVMTypeKind.LLVMDoubleTypeKind => op switch
                {
                    ASTType.Add => LLVM.BuildFAdd(builder, l, r, "addtmp"),
                    ASTType.Subtract => LLVM.BuildFSub(builder, l, r, "subtmp"),
                    ASTType.Multiply => LLVM.BuildFMul(builder, l, r, "multmp"),
                    ASTType.Divide => LLVM.BuildFDiv(builder, l, r, "divtmp"),
                    ASTType.Equal => LLVM.BuildFCmp(builder, LLVMRealPredicate.LLVMRealUEQ, l, r, "eqtmp"),
                    ASTType.LessThan => LLVM.BuildFCmp(builder, LLVMRealPredicate.LLVMRealULT, l, r, "lttmp"),
                    ASTType.LessEqual => LLVM.BuildFCmp(builder, LLVMRealPredicate.LLVMRealULE, l, r, "letmp"),
                    ASTType.GreaterThan => LLVM.BuildFCmp(builder, LLVMRealPredicate.LLVMRealUGT, l, r, "gttmp"),
                    ASTType.GreaterEqual => LLVM.BuildFCmp(builder, LLVMRealPredicate.LLVMRealUGE, l, r, "getmp"),
                    _ => throw new NotImplementedException()
                },
            };

        }
        public static LLVMTypeRef ToLLVM(this TypeKind ty) =>
            ty switch
            {
                TypeKind.Int => LLVM.Int64Type(),
                TypeKind.Float => LLVM.DoubleType(),
                TypeKind.Bool => LLVM.Int1Type(),
                TypeKind.Unit => LLVM.VoidType(),
                _ => throw new NotImplementedException()
            };
    }
}
