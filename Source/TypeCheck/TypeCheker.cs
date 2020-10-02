using System;
using System.Collections.Generic;
using System.Linq;
using Small_lang.AST;

namespace Small_lang.TypeCheck
{
    public class TypeChecker : ASTVisitor
    {
        public TypeChecker() { }

        private Dictionary<string, (ASTType, TypeKind)> typeTbl = new Dictionary<string, (ASTType, TypeKind)>();

        private Dictionary<string, (ASTType, List<TypedArg>, TypeKind)> fnTbl = 
            new Dictionary<string, (ASTType, List<TypedArg>, TypeKind)>();

        public List<BaseAST> ReorderAndCheck(List<BaseAST> exprs)
        {
            // reorder the AST
            exprs.Sort((e1, e2) => e1.NodeType.ASTValue() - e2.NodeType.ASTValue());
            var funs = exprs.TakeWhile(e => e.NodeType.ASTValue() < 0).ToList();
            // process func def
            this.Visit(funs);
            // process entry. clear the table first
            typeTbl.Clear();
            var main = exprs.SkipWhile(e => e.NodeType.ASTValue() < 0).ToList();
            this.Visit(main);
            return funs.Concat(main).ToList();
        }

        protected internal override void VisitAST(AssignStmtAST node)
        {
            if (fnTbl.ContainsKey(node.Name)) throw new TypeCheckException();

            this.Visit(node.Value);

            // Unit type is not allowed
            if (!node.Value.IsExpr) throw new TypeCheckException();

            if (typeTbl.TryGetValue(node.Name, out var type))
            {
                if (type.Item2 != node.Value.RetType)
                {
                    // throw error if type mismatch
                    throw new TypeCheckException();
                }
            } 
            else
            {
                typeTbl.Add(node.Name, (node.NodeType, node.Value.RetType));
            }
        }


        protected internal override void VisitAST(BinaryExprAST node)
        {
            this.Visit(node.Lhs);
            this.Visit(node.Rhs);
            // only expr can involve in binary operation
            if (!node.Lhs.IsExpr || !node.Rhs.IsExpr) throw new TypeCheckException();

            var widenType = (TypeKind)Math.Max((int)node.Lhs.RetType, (int)node.Rhs.RetType);
            var shrinkType = (TypeKind)Math.Min((int)node.Lhs.RetType, (int)node.Rhs.RetType);
            
            // bool mix number is not allowed
            if (widenType != TypeKind.Bool && shrinkType == TypeKind.Bool)
            {
                throw new TypeCheckException();
            }

            // and or need 2 bools
            if ((node.NodeType == ASTType.And || node.NodeType == ASTType.Or) && shrinkType != TypeKind.Bool)
            {
                throw new TypeCheckException();
            }
            
            if (node.NodeType.IsBoolOp())
            {
                node.RetType = TypeKind.Bool;
            }
            else
            {
                // type widen (int -> float)
                node.RetType = widenType;
                // float cannot do % operation
                if (node.NodeType == ASTType.Modulo && node.RetType == TypeKind.Float)
                {
                    throw new TypeCheckException();
                }

            }
        }

        protected internal override void VisitAST(BlockExprAST node)
        {
            this.Visit(node.Stmts);
            node.RetType = node.Stmts.Any() ? node.Stmts.Last().RetType : TypeKind.Unit;
        }

        protected internal override void VisitAST(BoolExprAST node)
        {
            node.RetType = TypeKind.Bool;
        }

        protected internal override void VisitAST(CallExprAST node)
        {
            this.Visit(node.Arguments);
            if (!fnTbl.TryGetValue(node.Callee, out var type)) throw new TypeCheckException();
            node.RetType = type.Item3;
            if (!type.Item2.Select(arg => arg.Type)
                        .SequenceEqual(node.Arguments.Select(arg => arg.RetType))
                ) throw new TypeCheckException();
        }

        protected internal override void VisitAST(FloatExprAST node)
        {
            node.RetType = TypeKind.Float;
        }

        protected internal override void VisitAST(IntExprAST node)
        {
            node.RetType = TypeKind.Int;
        }

        protected internal override void VisitAST(FuncStmtAST node)
        {
            this.Visit(node.Proto);
            this.Visit(node.Body);
            // body's return and declared return should be the same
            if (node.Body.RetType != node.Proto.FnRet) throw new TypeCheckException();
            // update the fn table
            fnTbl[node.Proto.Name] = (ASTType.Function, node.Proto.Arguments, node.Proto.FnRet);
        }

        protected internal override void VisitAST(IfStmtAST node)
        {
            var conds = node.Branches.Select(br => br.Cond).ToList();
            var bodies = node.Branches.Select(br => br.Body as BaseAST).ToList();
            var @else = node.ElseBranch;
            this.Visit(conds);
            foreach (var cond in conds)
            {
                // cond must be bool type
                if (cond.RetType != TypeKind.Bool) throw new TypeCheckException();
            }
            this.Visit(bodies);
            this.Visit(@else?.Body);
        }

        protected internal override void VisitAST(ProtoStmtAST node)
        {
            // if exists a func proto in fb table. the signature should be the same
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
            // clear the symbol table since entering a new scope
            typeTbl.Clear();
            foreach (var arg in node.Arguments)
            {
                if (fnTbl.ContainsKey(arg.Name)) throw new TypeCheckException();
                typeTbl.Add(arg.Name, (ASTType.Let, arg.Type));
            }
        }

        protected internal override void VisitAST(VariableExprAST node)
        {
            // check exists
            if (!typeTbl.TryGetValue(node.Name, out var type)) throw new TypeCheckException();
            node.RetType = type.Item2;
        }

        protected internal override void VisitAST(ReadStmtAST node)
        {
            // similar with CheckAssignment AST
            if (fnTbl.ContainsKey(node.Name)) throw new TypeCheckException();

            if (typeTbl.TryGetValue(node.Name, out var type))
            {
                if (type.Item2 != node.RetType)
                {
                    // throw error if type mismatch
                    throw new TypeCheckException();
                }
            }
            else
            {
                typeTbl.Add(node.Name, (node.NodeType, node.VarType));
            }
        }

        protected internal override void VisitAST(WriteStmtAST node)
        {
            this.Visit(node.Value);
        }

        protected internal override void VisitAST(ForStmtAST node)
        {
            this.Visit(node.PreRun);
            this.Visit(node.PostRun);
            var cond = node.InfLoop.Cond;
            var body = node.InfLoop.Body;

            this.Visit(cond);
            if (cond.RetType != TypeKind.Bool) throw new TypeCheckException();
            this.Visit(body);
        }
        
        protected internal override void VisitAST(RepeatStmtAST node)
        {
            var cond = node.InfLoop.Cond;
            var body = node.InfLoop.Body;

            this.Visit(cond);
            if (cond.RetType != TypeKind.Bool) throw new TypeCheckException();
            this.Visit(body);
        }

        protected internal override void VisitAST(UnaryExprAST node)
        {
            this.Visit(node.Hs);
            if (!node.Hs.IsExpr) throw new TypeCheckException();
            node.RetType = node.NodeType switch
            {
                ASTType.Not when node.Hs.RetType != TypeKind.Bool => throw new TypeCheckException(),
                ASTType.Neg when node.Hs.RetType == TypeKind.Bool => throw new TypeCheckException(),
                _ => node.Hs.RetType
            };
        }
    }
}
