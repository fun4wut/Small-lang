using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Kumiko_lang.AST
{
    public class TypeChecker
    {
        public TypeChecker() { }

        Dictionary<string, (ASTType, TypeKind)> typeTbl = new Dictionary<string, (ASTType, TypeKind)>();

        Dictionary<string, (ASTType, List<TypedArg>, TypeKind)> fnTbl = 
            new Dictionary<string, (ASTType, List<TypedArg>, TypeKind)>();

        private void ThrowsUnless<T>(Func<bool> action) where T: Exception, new()
        {
            if (!action()) throw new T();
        }

        public void Check(BaseAST node) => node.CheckWith(this);

        public void Check(List<BaseAST> nodes) => nodes.ForEach(stmt => this.Check(stmt));

        public void CheckAST(AssignStmtAST stmt) => ThrowsUnless<TypeCheckException>(() =>
        {
            if (!typeTbl.TryGetValue(stmt.Name, out var type)) return false;
            this.Check(stmt.Value);
            return type.Item1 == ASTType.Mut
            && type.Item2 != TypeKind.Unit
            && stmt.Value.RetType == type.Item2;
        });


        public void CheckAST(BinaryExprAST expr) => ThrowsUnless<TypeCheckException>(() =>
        {
            this.Check(expr.Lhs);
            this.Check(expr.Rhs);
            if (!expr.Lhs.IsExpr || !expr.Rhs.IsExpr) return false;
            if (expr.NodeType.IsBoolOp())
            {
                expr.RetType = TypeKind.Bool;
                if (expr.Lhs.RetType != TypeKind.Bool || expr.Rhs.RetType != TypeKind.Bool)
                {
                    if (expr.NodeType != ASTType.Equal) return false;
                }
            }
            else
            {
                expr.RetType = (TypeKind)Math.Max((int)expr.Lhs.RetType, (int)expr.Rhs.RetType);
            }
            return true;
        });

        public void CheckAST(BlockExprAST expr) => ThrowsUnless<TypeCheckException>(() =>
        {
            this.Check(expr.Stmts);
            expr.RetType = expr.Stmts.Last().RetType;
            return true;
        });

        public void CheckAST(BoolExprAST expr) => ThrowsUnless<TypeCheckException>(() =>
        {
            expr.RetType = TypeKind.Bool;
            return true;
        });

        public void CheckAST(CallExprAST expr) => ThrowsUnless<TypeCheckException>(() =>
        {
            this.Check(expr.Arguments);
            if (!fnTbl.TryGetValue(expr.Callee, out var fnRet)) return false;
            expr.RetType = fnRet.Item3;
            return true;
        });

        public void CheckAST(DeclStmtAST stmt) => ThrowsUnless<TypeCheckException>(() =>
        {
            this.Check(stmt.Value);
            if (!typeTbl.TryAdd(stmt.Name, (stmt.NodeType, stmt.Value.RetType))) return false;
            return stmt.Value.IsExpr;
        });

        public void CheckAST(FloatExprAST expr) => ThrowsUnless<TypeCheckException>(() =>
        {
            expr.RetType = TypeKind.Float;
            return true;
        });

        public void CheckAST(IntExprAST expr) => ThrowsUnless<TypeCheckException>(() =>
        {
            expr.RetType = TypeKind.Int;
            return true;
        });

        public void CheckAST(FuncStmtAST stmt) => ThrowsUnless<TypeCheckException>(() =>
        {
            this.Check(stmt.Proto);
            this.Check(stmt.Body);
            typeTbl[stmt.Proto.Name] = (stmt.NodeType, stmt.Proto.FnRet);
            return true;
        });

        public void CheckAST(IfExprAST expr) => ThrowsUnless<TypeCheckException>(() =>
        {
            var conds = expr.Branches.Select(br => br.Cond).ToList();
            var bodies = expr.Branches.Select(br => br.Body as BaseAST).ToList();
            var @else = expr.ElseBranch;
            this.Check(conds);
            this.Check(bodies);
            if (@else?.Body is BaseAST elseBody)
            {
                this.Check(elseBody);
                foreach (var item in bodies)
                {
                    // if not all the branched return the same type. this expr will be stmt
                    if (item.RetType != elseBody.RetType) return true;
                }
                expr.RetType = elseBody.RetType;
            }
            return true;
        });

        public void CheckAST(ProtoStmtAST stmt) => ThrowsUnless<TypeCheckException>(() =>
        {
            if (fnTbl.TryGetValue(stmt.Name, out var type))
            {
                // redefined function
                if (type.Item1 == ASTType.Function) return false;
                
                // args type doesn't match
                if (!type.Item2.Select(arg => arg.Type)
                        .SequenceEqual(stmt.Arguments.Select(arg => arg.Type))
                ) return false;

                // ret type doesn't match
                if (type.Item3 == stmt.FnRet) return false;
            }
            else
            {
                fnTbl.Add(stmt.Name, (ASTType.Prototype, stmt.Arguments, stmt.FnRet));
            }

            typeTbl.Clear();
            foreach (var arg in stmt.Arguments)
            {
                typeTbl.Add(arg.Name, (ASTType.Let, arg.Type));
            }

            return true;
        });

        public void CheckAST(VariableExprAST expr) => ThrowsUnless<TypeCheckException>(() =>
        {
            if (!typeTbl.TryGetValue(expr.Name, out var type)) return false;
            expr.RetType = type.Item2;
            return true;
        });


    }
}
