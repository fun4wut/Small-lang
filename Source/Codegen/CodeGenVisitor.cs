using System;
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

        private readonly Dictionary<string, LLVMValueRef> symTbl = new Dictionary<string, LLVMValueRef>();

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
            if (this.symTbl.ContainsKey(node.Name))
            {
                throw new DupDeclException();
            }
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
            var argCnt = (uint)node.Arguments.Count;
            var args = new LLVMTypeRef[Math.Max(argCnt, 1)];
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
            }
            else
            {
                for (int i = 0; i < argCnt; ++i)
                {
                    args[i] = node.Arguments[i].Type.ToLLVM();
                }

                function = LLVM.AddFunction(
                    this.module, node.Name, LLVM.FunctionType(LLVM.DoubleType(), args, false)
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

            return node;
        }

        protected internal override ExprAST VisitAST(FuncExprAST node)
        {
            return base.VisitAST(node);
        }
    }

}
