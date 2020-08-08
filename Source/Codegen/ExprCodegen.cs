using System;
using System.Collections.Generic;
using System.Text;
using LLVMSharp;
using Kumiko_lang.AST;
using System.Linq;

namespace Kumiko_lang.Codegen
{
    public partial class CodeGenVisitor : ExprVisitor
    {
        protected internal override BaseAST VisitAST(BinaryExprAST node)
        {
            this.Visit(node.Lhs);
            this.Visit(node.Rhs);

            LLVMValueRef r = this.LatestValue(), l = this.LatestValue();

            var n = this.builder.DoBinaryOps(node.NodeType, l, r);
            this.ResultStack.Push(n);
            return node;
        }

        protected internal override BaseAST VisitAST(FloatExprAST node)
        {
            this.ResultStack.Push(LLVM.ConstReal(LLVM.DoubleType(), node.Value));
            return node;
        }

        protected internal override BaseAST VisitAST(IntExprAST node)
        {
            this.ResultStack.Push(LLVM.ConstInt(LLVM.Int64Type(), (ulong)node.Value, true));
            return node;
        }

        protected internal override BaseAST VisitAST(VariableExprAST node)
        {
            if (this.symTbl.TryGetValue(node.Name, out LLVMValueRef value))
            {
                this.ResultStack.Push(value);
            }
            else
            {
                throw new UndefinedVarException();
            }
            return node;
        }

        protected internal override BaseAST VisitAST(IfExprAST node)
        {
            // get the current fn
            var fn = LLVM.GetInsertBlock(this.builder).GetBasicBlockParent();
            var thenBB = LLVM.AppendBasicBlock(fn, "then");
            var mergeBB = LLVM.AppendBasicBlock(fn, "ifcont");

            // emit merge code
            LLVM.PositionBuilderAtEnd(this.builder, mergeBB);
            // lazily build phi node
            LLVMValueRef? phi = null;

            // recursly gen the if-else
            this.BuildCond(ref phi, node.Branches, ref thenBB, mergeBB);

            return node;
        }

        protected internal override BaseAST VisitAST(CallExprAST node)
        {
            var calleeF = LLVM.GetNamedFunction(this.module, node.Callee);
            if (calleeF.Pointer == IntPtr.Zero)
            {
                throw new Exception("Unknown function referenced");
            }

            if (LLVM.CountParams(calleeF) != node.Arguments.Count)
            {
                throw new Exception("Incorrect # arguments passed");
            }

            var argsCnt = (uint)node.Arguments.Count;
            var argsV = new LLVMValueRef[argsCnt];
            for (int i = 0; i < argsCnt; ++i)
            {
                this.Visit(node.Arguments[i]);
                argsV[i] = this.LatestValue();
            }

            var fnRet = calleeF.TypeOf().GetReturnType().GetReturnType().TypeKind
                == LLVMTypeKind.LLVMVoidTypeKind ? string.Empty : "calltmp";

            this.ResultStack.Push(LLVM.BuildCall(this.builder, calleeF, argsV, fnRet));

            return node;
        }
    }
}
