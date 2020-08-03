﻿using System;
using System.Collections.Generic;
using System.Text;
using LLVMSharp;
using Kumiko_lang.AST;

namespace Kumiko_lang.Codegen
{
    public partial class CodeGenVisitor : ExprVisitor
    {
        private static readonly LLVMBool LLVMBoolFalse = new LLVMBool(0);

        private static readonly LLVMValueRef NullValue = new LLVMValueRef(IntPtr.Zero);

        private readonly LLVMModuleRef module;

        private readonly LLVMBuilderRef builder;

        private Dictionary<string, LLVMValueRef> symTbl = new Dictionary<string, LLVMValueRef>();

        private HashSet<string> fnSet = new HashSet<string>();

        private Dictionary<string, LLVMValueRef>? tmpTbl = null;

        public CodeGenVisitor(LLVMModuleRef module, LLVMBuilderRef builder)
        {
            this.module = module;
            this.builder = builder;
        }

        public Stack<LLVMValueRef> ResultStack { get; } = new Stack<LLVMValueRef>();

        public void ClearResultStack()
        {
            this.ResultStack.Clear();
        }

        public string PrintTop() => this.ResultStack.Pop().PrintValueToString().Trim();

        void CheckNoDup(AssignExprAST node)
        {
            if (this.symTbl.ContainsKey(node.Name) || this.fnSet.Contains(node.Name))
            {
                throw new DupDeclException();
            }
        }

        void CheckNoDup(ProtoExprAST node)
        {
            if (this.symTbl.ContainsKey(node.Name))
            {
                throw new DupDeclException();
            }
        }

        protected internal override ExprAST VisitAST(BinaryExprAST node)
        {
            this.Visit(node.Lhs);
            this.Visit(node.Rhs);

            LLVMValueRef r = this.ResultStack.Pop(), l = this.ResultStack.Pop();
            
            var n = node.NodeType switch
            {
                ExprType.AddExpr => LLVM.BuildAdd(this.builder, l, r, "addtmp"),
                ExprType.SubtractExpr => LLVM.BuildSub(this.builder, l, r, "subtmp"),
                ExprType.MultiplyExpr => LLVM.BuildMul(this.builder, l, r, "multmp"),
                ExprType.DivideExpr => LLVM.BuildSDiv(this.builder, l, r, "divtmp"),
                _ => throw new NotImplementedException()
            };
            this.ResultStack.Push(n);
            return node;
        }

        protected internal override ExprAST VisitAST(AssignExprAST node)
        {
            this.CheckNoDup(node);
            this.Visit(node.Value);
            var top = this.ResultStack.Pop();
            top.SetValueName(node.Name);
            this.symTbl.Add(node.Name, top);
            return node;
        }

        protected internal override ExprAST VisitAST(FloatExprAST node)
        {
            this.ResultStack.Push(LLVM.ConstReal(LLVM.DoubleType(), node.Value));
            return node;
        }

        protected internal override ExprAST VisitAST(IntExprAST node)
        {
            this.ResultStack.Push(LLVM.ConstInt(LLVM.Int64Type(), (ulong)node.Value, true));
            return node;
        }

        protected internal override ExprAST VisitAST(VariableExprAST node)
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

        protected internal override ExprAST VisitAST(ProtoExprAST node)
        {
            this.CheckNoDup(node);
            var argCnt = (uint)node.Arguments.Count;
            var args = new LLVMTypeRef[argCnt];
            var function = LLVM.GetNamedFunction(this.module, node.Name);

            // If F conflicted, there was already something named 'Name'.  If it has a
            // body, don't allow redefinition
            if (function.Pointer != IntPtr.Zero)
            {
                // If F already has a body, reject this.
                if (LLVM.CountBasicBlocks(function) != 0)
                {
                    throw new Exception("redefinition of function.");
                }

                // If F took a different number of args, reject.
                if (LLVM.CountParams(function) != argCnt)
                {
                    throw new Exception("redefinition of function with different # args");
                }

                // If F return different type, reject.
                if (function.TypeOf().GetReturnType().GetReturnType().TypeKind != node.RetType.ToLLVM().TypeKind)
                {
                    throw new DupDeclException();
                }
            }
            else
            {
                for (int i = 0; i < argCnt; ++i)
                {
                    args[i] = node.Arguments[i].Type.ToLLVM();
                }

                function = LLVM.AddFunction(
                    this.module, node.Name, LLVM.FunctionType(node.RetType.ToLLVM(), args, false)
                );

                LLVM.SetLinkage(function, LLVMLinkage.LLVMExternalLinkage);
            }


            for (int i = 0; i < argCnt; ++i)
            {
                string argName = node.Arguments[i].Name;

                LLVMValueRef param = LLVM.GetParam(function, (uint)i);
                LLVM.SetValueName(param, argName);

                this.symTbl[argName] = param;
            }

            this.ResultStack.Push(function);
            this.fnSet.Add(node.Name);
            return node;
        }

        protected internal override ExprAST VisitAST(FuncExprAST node)
        {
            this.tmpTbl = this.symTbl;
            this.symTbl = new Dictionary<string, LLVMValueRef>();
            this.Visit(node.Proto);
            LLVMValueRef function = this.ResultStack.Pop();
            
            // Create a new basic block to start insertion into.
            LLVM.PositionBuilderAtEnd(this.builder, LLVM.AppendBasicBlock(function, "entry"));
            
            try
            {
                // node.Body.Compile(this);
                node.Body.ForEach(exp => this.Visit(exp));
            }
            catch (Exception)
            {
                LLVM.DeleteFunction(function);
                throw;
            }

            // Finish off the function.
            LLVM.BuildRet(this.builder, this.ResultStack.Pop());

            // Validate the generated code, checking for consistency.
            LLVM.VerifyFunction(function, LLVMVerifierFailureAction.LLVMPrintMessageAction);

            this.ResultStack.Push(function);
            this.symTbl = this.tmpTbl;
            return node;
        }
    }

}
