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

        protected internal override BaseAST VisitAST(BoolExprAST node)
        {
            this.ResultStack.Push(LLVM.ConstInt(LLVM.Int1Type(), node.Value ? 1u : 0u, true));
            return node;
        }

        protected internal override BaseAST VisitAST(IntExprAST node)
        {
            this.ResultStack.Push(LLVM.ConstInt(LLVM.Int64Type(), (ulong)node.Value, true));
            return node;
        }

        protected internal override BaseAST VisitAST(VariableExprAST node)
        {
            var value = this.symTbl[node.Name];
            this.ResultStack.Push(value);
            return node;
        }

        protected internal override BaseAST VisitAST(IfExprAST node)
        {
            var initialBB = this.GetCurrentBB();
            // get the current fn
            var fn = initialBB.GetBasicBlockParent();
            var thenBB = LLVM.AppendBasicBlock(fn, "then");
            var mergeBB = LLVM.AppendBasicBlock(fn, "ifcont");
            // emit merge code
            LLVM.PositionBuilderAtEnd(this.builder, mergeBB);

            LLVMValueRef? phi = null;

            // if expr
            if (node.IsExpr)
            {
                phi = LLVM.BuildPhi(this.builder, node.RetType.ToLLVM(), "phi");
                this.ResultStack.Push((LLVMValueRef)phi);
            }

            // move the builder to the initial postion
            LLVM.PositionBuilderAtEnd(this.builder, initialBB);
            // recursly gen the if-else
            this.BuildCond(phi, node.Branches, node.ElseBranch, ref thenBB, mergeBB);
            
            // move builder to end
            LLVM.PositionBuilderAtEnd(this.builder, mergeBB);

            return node;
        }

        protected internal override BaseAST VisitAST(CallExprAST node)
        {
            var calleeF = LLVM.GetNamedFunction(this.module, node.Callee);

            var argsCnt = (uint)node.Arguments.Count;
            var argsV = new LLVMValueRef[argsCnt];
            for (int i = 0; i < argsCnt; ++i)
            {
                this.Visit(node.Arguments[i]);
                argsV[i] = this.LatestValue();
            }

            var fnRet = checker.fnTbl[node.Callee].Item3 == TypeKind.Unit ? string.Empty : "calltmp";

            this.ResultStack.Push(LLVM.BuildCall(this.builder, calleeF, argsV, fnRet));

            return node;
        }
    }
}
