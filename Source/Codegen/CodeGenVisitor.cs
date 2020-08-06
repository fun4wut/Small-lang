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

        private Dictionary<string, LLVMValueRef> symTbl = new Dictionary<string, LLVMValueRef>();

        private HashSet<string> fnSet = new HashSet<string>();

        private Dictionary<string, LLVMValueRef>? tmpTbl = null;

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

            // func main is reserved
            this.fnSet.Add("main");
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

        protected internal override ExprAST VisitAST(BinaryExprAST node)
        {
            this.Visit(node.Lhs);
            this.Visit(node.Rhs);

            LLVMValueRef r = this.LatestValue(), l = this.LatestValue();
            
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

        protected internal override ExprAST VisitAST(DeclExprAST node)
        {
            this.CheckNoDup(node.Name);
            this.Visit(node.Value);
            var top = this.LatestValue();
            // let immutable
            if (node.NodeType == ExprType.LetExpr)
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

        protected internal override ExprAST VisitAST(ProtoExprAST node, bool combineUse = false)
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

        protected internal override ExprAST VisitAST(FuncExprAST node, bool isMain = false)
        {
            this.ReplaceTbl();
            this.VisitAST(node.Proto, combineUse: true);

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
            LLVM.BuildRet(this.builder, 
                isMain
                    ? LLVM.ConstInt(LLVM.Int64Type(), 0, true) 
                    : this.LatestValue()
            );
            

            // Validate the generated code, checking for consistency.
            LLVM.VerifyFunction(function, LLVMVerifierFailureAction.LLVMPrintMessageAction);

            this.ResultStack.Push(function);

            this.RestoreTbl();

            return node;
        }

        protected internal override ExprAST VisitAST(CallExprAST node)
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

        public void InsertMain(IEnumerable<ExprAST> exprs)
        {
            // init main function
            this.VisitAST(
                new FuncExprAST(
                    new ProtoExprAST(
                        "main",
                        new List<TypedArg>(),
                        TypeEnum.Int
                    ),
                    exprs
                ),
                isMain: true
            );
            this.MainFn = this.ResultStack.Pop();
        }

    }

}
