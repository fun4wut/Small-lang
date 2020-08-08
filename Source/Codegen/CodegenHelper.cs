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
        #region Helper method
        private void RestoreTbl()
        {
            this.symTbl = this.tmpTbl ?? this.symTbl;
            this.tmpTbl = null;
        }

        private void ReplaceTbl()
        {
            this.tmpTbl = this.symTbl;
            this.symTbl = new Dictionary<string, LLVMValueRef>();
        }

        private LLVMValueRef LatestValue()
        {
            var top = this.ResultStack.Pop();
            return top.IsPtr()
                ? LLVM.BuildLoad(this.builder, top, "ptrtmp")
                : top;
        }

        public CodeGenVisitor(LLVMModuleRef module, LLVMBuilderRef builder)
        {
            this.module = module;
            this.builder = builder;
        }

        public LLVMValueRef MainFn { get; private set; }

        public Stack<LLVMValueRef> ResultStack { get; } = new Stack<LLVMValueRef>();

        public void ClearResultStack()
        {
            this.ResultStack.Clear();
        }

        public string PrintTop() => this.ResultStack.Pop().PrintValueToString().Trim();

        void CheckNoDup(string name, bool isFn = false)
        {
            if (isFn && this.symTbl.ContainsKey(name))
            {
                throw new DupDeclException();
            }
            if (!isFn && (this.symTbl.ContainsKey(name) || this.fnSet.Contains(name)))
            {
                throw new DupDeclException();
            }
        }

        void BuildCond(
            ref LLVMValueRef? phi,
            IEnumerable<Branch> branches, 
            ref LLVMBasicBlockRef thenBB, 
            LLVMBasicBlockRef mergeBB
        )
        {
            if (!branches.Any())
            {
                return;
            }

            var @if = branches.First();
            this.Visit(@if.Cond);
            // if cond
            var top = this.LatestValue();
            // set value for phi
            if (phi == null)
            {
                phi = LLVM.BuildPhi(this.builder, top.TypeOf(), "phi");
                this.ResultStack.Push((LLVMValueRef)phi!);
            }
            if (!top.isBool())
            {
                throw new Exception("only bool can used in cond");
            }
            top.SetValueName("ifcond");

            // insert elseBB after thenBB
            var elseBB = LLVM.InsertBasicBlock(thenBB, "else");

            // build cond br
            LLVM.BuildCondBr(this.builder, top, thenBB, elseBB);

            // move the builder to then block
            LLVM.PositionBuilderAtEnd(this.builder, thenBB);
            
            // codegen
            @if.Body.Stmts.ForEach(stmt => this.Visit(stmt));
            LLVM.BuildBr(this.builder, mergeBB);
            // Codegen of 'Then' can change the current block, update ThenBB for the PHI.
            thenBB = LLVM.GetInsertBlock(this.builder);

            phi?.AddIncoming(
                new LLVMValueRef[] { this.LatestValue() },
                new LLVMBasicBlockRef[] { thenBB },
                1
            );

            this.BuildCond(ref phi, branches.Skip(1), ref elseBB, mergeBB);

        }

        #endregion
    }
}
