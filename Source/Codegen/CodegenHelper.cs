using System;
using System.Collections.Generic;
using System.Text;
using LLVMSharp;
using Kumiko_lang.AST;
using System.Linq;
using Kumiko_lang.TypeCheck;

namespace Kumiko_lang.Codegen
{
    public partial class CodeGenVisitor : ExprVisitor
    {
        #region Helper method

        private LLVMValueRef LatestValue()
        {
            var top = this.ResultStack.Pop();
            return top.IsPtr()
                ? LLVM.BuildLoad(this.builder, top, "ptrtmp")
                : top;
        }

        public CodeGenVisitor(LLVMModuleRef module, LLVMBuilderRef builder, TypeChecker checker)
        {
            this.module = module;
            this.builder = builder;
            this.checker = checker;
        }

        public string PrintTop() => this.ResultStack.Pop().PrintValueToString().Trim();

        void BuildCond(
            LLVMValueRef phi,
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
            this.Visit(@if.Body.Stmts);
            LLVM.BuildBr(this.builder, mergeBB);
            // Codegen of 'Then' can change the current block, update ThenBB for the PHI.
            thenBB = LLVM.GetInsertBlock(this.builder);

            phi.AddIncoming(
                new LLVMValueRef[] { this.LatestValue() },
                new LLVMBasicBlockRef[] { thenBB },
                1
            );

            this.BuildCond(phi, branches.Skip(1), ref elseBB, mergeBB);

        }

        #endregion
    }
}
