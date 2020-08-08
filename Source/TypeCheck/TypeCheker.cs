using System;
using System.Collections.Generic;
using System.Linq;
using Kumiko_lang.AST;

namespace Kumiko_lang.TypeCheck
{
    public class TypeChecker
    {
        public TypeChecker() { }

        public Dictionary<string, (ASTType, TypeKind)> typeTbl = new Dictionary<string, (ASTType, TypeKind)>();

        public Dictionary<string, (ASTType, List<TypedArg>, TypeKind)> fnTbl = 
            new Dictionary<string, (ASTType, List<TypedArg>, TypeKind)>();

        public List<BaseAST> ReorderAndCheck(List<BaseAST> exprs)
        {
            // reorder the AST
            exprs.Sort((e1, e2) => e1.NodeType.ASTValue() - e2.NodeType.ASTValue());
            var funs = exprs.TakeWhile(e => e.NodeType.ASTValue() < 0);
            var main = exprs.SkipWhile(e => e.NodeType.ASTValue() < 0).MakeMain();
            var program = funs.Append(main).ToList();
            this.Check(program);
            return program;
        }

        public void Check(BaseAST node) => node.CheckWith(this);

        public void Check(List<BaseAST> nodes) => nodes.ForEach(node => this.Check(node));

        public void CheckAST(AssignStmtAST node)
        {
            if (!typeTbl.TryGetValue(node.Name, out var type)) throw new TypeCheckException();
            this.Check(node.Value);
            if (type.Item1 != ASTType.Mut
                || type.Item2 == TypeKind.Unit
                || node.Value.RetType != type.Item2) throw new TypeCheckException();
        }


        public void CheckAST(BinaryExprAST node)
        {
            this.Check(node.Lhs);
            this.Check(node.Rhs);
            if (!node.Lhs.IsExpr || !node.Rhs.IsExpr) throw new TypeCheckException();
            if (node.NodeType.IsBoolOp())
            {
                node.RetType = TypeKind.Bool;
                if (node.NodeType == ASTType.Equal)
                {
                    if (node.Lhs.RetType != node.Rhs.RetType) throw new TypeCheckException();
                }
                else
                {
                    if (node.Lhs.RetType == TypeKind.Bool || node.Rhs.RetType == TypeKind.Bool)
                    {
                        throw new TypeCheckException();
                    }
                }
            }
            else
            {
                node.RetType = (TypeKind)Math.Max((int)node.Lhs.RetType, (int)node.Rhs.RetType);
            }
        }

        public void CheckAST(BlockExprAST node)
        {
            this.Check(node.Stmts);
            node.RetType = node.Stmts.Last().RetType;
        }

        public void CheckAST(BoolExprAST node)
        {
            node.RetType = TypeKind.Bool;
        }

        public void CheckAST(CallExprAST node)
        {
            this.Check(node.Arguments);
            if (!fnTbl.TryGetValue(node.Callee, out var type)) throw new TypeCheckException();
            node.RetType = type.Item3;
            if (!type.Item2.Select(arg => arg.Type)
                        .SequenceEqual(node.Arguments.Select(arg => arg.RetType))
                ) throw new TypeCheckException();
        }

        public void CheckAST(DeclStmtAST node)
        {
            this.Check(node.Value);
            if (fnTbl.ContainsKey(node.Name)) throw new TypeCheckException();
            if (!typeTbl.TryAdd(node.Name, (node.NodeType, node.Value.RetType))) throw new TypeCheckException();
            if (!node.Value.IsExpr) throw new TypeCheckException();
        }

        public void CheckAST(FloatExprAST node)
        {
            node.RetType = TypeKind.Float;
        }

        public void CheckAST(IntExprAST node)
        {
            node.RetType = TypeKind.Int;
        }

        public void CheckAST(FuncStmtAST node)
        {
            this.Check(node.Proto);
            this.Check(node.Body);
            fnTbl[node.Proto.Name] = (node.NodeType, node.Proto.Arguments, node.Proto.FnRet);
        }

        public void CheckAST(IfExprAST node)
        {
            var conds = node.Branches.Select(br => br.Cond).ToList();
            var bodies = node.Branches.Select(br => br.Body as BaseAST).ToList();
            var @else = node.ElseBranch;
            this.Check(conds);
            this.Check(bodies);
            if (@else?.Body is BaseAST elseBody)
            {
                this.Check(elseBody);
                foreach (var item in bodies)
                {
                    // if not all the branched return the same type. this node will be node
                    if (item.RetType != elseBody.RetType) throw new TypeCheckException();
                }
                node.RetType = elseBody.RetType;
            }
        }

        public void CheckAST(ProtoStmtAST node)
        {
            if (fnTbl.TryGetValue(node.Name, out var type))
            {
                // redefined function
                if (type.Item1 == ASTType.Function) throw new TypeCheckException();

                // args type doesn't match
                if (!type.Item2.Select(arg => arg.Type)
                        .SequenceEqual(node.Arguments.Select(arg => arg.Type))
                ) throw new TypeCheckException();

                // ret type doesn't match
                if (type.Item3 != node.FnRet) throw new TypeCheckException();
            }
            else
            {
                fnTbl.Add(node.Name, (ASTType.Prototype, node.Arguments, node.FnRet));
            }

            typeTbl.Clear();
            foreach (var arg in node.Arguments)
            {
                if (fnTbl.ContainsKey(arg.Name)) throw new TypeCheckException();
                typeTbl.Add(arg.Name, (ASTType.Let, arg.Type));
            }
        }

        public void CheckAST(VariableExprAST node)
        {
            if (!typeTbl.TryGetValue(node.Name, out var type)) throw new TypeCheckException();
            node.RetType = type.Item2;
        }


    }
}
