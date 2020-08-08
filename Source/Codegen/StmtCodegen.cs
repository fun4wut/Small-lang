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
        #region Members
        private static readonly LLVMBool LLVMBoolFalse = new LLVMBool(0);

        private static readonly LLVMValueRef NullValue = new LLVMValueRef(IntPtr.Zero);

        private readonly LLVMModuleRef module;

        private readonly LLVMBuilderRef builder;

        private Dictionary<string, LLVMValueRef> symTbl = new Dictionary<string, LLVMValueRef>();

        private HashSet<string> fnSet = new HashSet<string>();

        private Dictionary<string, LLVMValueRef>? tmpTbl = null;

        #endregion

        #region Stmt Override
        protected internal override BaseAST VisitAST(DeclStmtAST node)
        {
            this.CheckNoDup(node.Name);
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


        protected internal override BaseAST VisitAST(ProtoStmtAST node, bool combineUse = false)
        {
            this.CheckNoDup(node.Name, isFn: true);

            if (!combineUse)
            {
                this.ReplaceTbl();
            }

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
                if (function.TypeOf().GetReturnType().GetReturnType().TypeKind != node.FnRet.ToLLVM().TypeKind)
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
                    this.module, node.Name, LLVM.FunctionType(node.FnRet.ToLLVM(), args, false)
                );

                LLVM.SetLinkage(function, LLVMLinkage.LLVMExternalLinkage);
            }

            this.fnSet.Add(node.Name);

            for (int i = 0; i < argCnt; ++i)
            {
                string argName = node.Arguments[i].Name;

                LLVMValueRef param = LLVM.GetParam(function, (uint)i);
                LLVM.SetValueName(param, argName);

                this.CheckNoDup(argName);
                this.symTbl[argName] = param;
            }

            this.ResultStack.Push(function);

            if (!combineUse)
            {
                this.RestoreTbl();
            }

            return node;
        }

        protected internal override BaseAST VisitAST(FuncStmtAST node)
        {
            this.ReplaceTbl();
            this.VisitAST(node.Proto, combineUse: true);

            LLVMValueRef function = this.ResultStack.Pop();
            
            // Create a new basic block to start insertion into.
            LLVM.PositionBuilderAtEnd(this.builder, LLVM.AppendBasicBlock(function, "entry"));
            
            try
            {
                // node.Body.Compile(this);
                node.Body.Stmts.ForEach(exp => this.Visit(exp));
            }
            catch (Exception)
            {
                LLVM.DeleteFunction(function);
                throw;
            }

            // Finish off the function.
            LLVM.BuildRet(this.builder, this.LatestValue());
            

            // Validate the generated code, checking for consistency.
            LLVM.VerifyFunction(function, LLVMVerifierFailureAction.LLVMPrintMessageAction);

            if (function.GetValueName() == "main") this.MainFn = function;

            this.ResultStack.Push(function);

            this.RestoreTbl();

            return node;
        }

        

        protected internal override BaseAST VisitAST(AssignStmtAST node)
        {
            if (this.symTbl.TryGetValue(node.Name, out var ptr))
            {
                if (ptr.IsPtr())
                {
                    this.Visit(node.Value);
                    var val = this.LatestValue();
                    LLVM.BuildStore(this.builder, val, ptr);
                }
                else
                {
                    throw new Exception("cannot mutate the immut");
                }
            }
            else
            {
                throw new UndefinedVarException();
            }
            return node;
        }

        #endregion

    }

}
