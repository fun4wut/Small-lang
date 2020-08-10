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
        #region Members
        public LLVMValueRef MainFn { get; private set; }
        public Stack<LLVMValueRef> ResultStack { get; } = new Stack<LLVMValueRef>();

        private static readonly LLVMBool LLVMBoolFalse = new LLVMBool(0);

        private static readonly LLVMValueRef NullValue = new LLVMValueRef(IntPtr.Zero);

        private readonly LLVMModuleRef module;

        private readonly LLVMBuilderRef builder;

        private Dictionary<string, LLVMValueRef> symTbl = new Dictionary<string, LLVMValueRef>();

        private TypeChecker checker;
        #endregion

        #region Stmt Override
        protected internal override BaseAST VisitAST(DeclStmtAST node)
        {
            this.Visit(node.Value);
            var top = this.LatestValue();
            // let immutable
            if (node.NodeType == ASTType.Let)
            {
                top.SetValueName(node.Name);
                this.symTbl.Add(node.Name, top);
            }
            // mutable
            else
            {
                var alloca = LLVM.BuildAlloca(this.builder, top.TypeOf(), node.Name);
                LLVM.BuildStore(this.builder, top, alloca);
                this.symTbl.Add(node.Name, alloca);
            }
            return node;
        }


        protected internal override BaseAST VisitAST(ProtoStmtAST node)
        {
            this.symTbl.Clear();
            var argCnt = (uint)node.Arguments.Count;
            var args = new LLVMTypeRef[argCnt];
            var function = LLVM.GetNamedFunction(this.module, node.Name);

            for (int i = 0; i < argCnt; ++i)
            {
                args[i] = node.Arguments[i].Type.ToLLVM();
            }

            function = LLVM.AddFunction(
                this.module, node.Name, LLVM.FunctionType(node.FnRet.ToLLVM(), args, false)
            );

            LLVM.SetLinkage(function, LLVMLinkage.LLVMExternalLinkage);
            

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

        protected internal override BaseAST VisitAST(FuncStmtAST node)
        {
            this.VisitAST(node.Proto);

            LLVMValueRef function = this.ResultStack.Pop();
            
            // Create a new basic block to start insertion into.
            LLVM.PositionBuilderAtEnd(this.builder, LLVM.AppendBasicBlock(function, "entry"));

            this.Visit(node.Body.Stmts);
            

            // Finish off the function.
            LLVM.BuildRet(this.builder, this.LatestValue());
            
            // Validate the generated code, checking for consistency.
            LLVM.VerifyFunction(function, LLVMVerifierFailureAction.LLVMPrintMessageAction);

            if (function.GetValueName() == "main") this.MainFn = function;

            this.ResultStack.Push(function);

            return node;
        }

        

        protected internal override BaseAST VisitAST(AssignStmtAST node)
        {
            var ptr = this.symTbl[node.Name];
            this.Visit(node.Value);
            var val = this.LatestValue();
            LLVM.BuildStore(this.builder, val, ptr);
            return node;
        }

        #endregion

    }

}
