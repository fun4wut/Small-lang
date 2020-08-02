using System;
using System.Collections.Generic;
using System.Text;
using LLVMSharp;
using Kumiko_lang.AST;

namespace Kumiko_lang.Codegen
{
    public class CodeGenVisitor : ExprVisitor
    {
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
                throw new Exception("Duplicate Variable name");
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
                throw new Exception("Unknown variable name");
            }
            return node;
        }
    }
}
