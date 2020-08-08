using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Kumiko_lang.AST
{
    public class TypeCheker
    {
        protected TypeCheker() { }

        Dictionary<string, (ASTType, TypeKind)> typeTbl = new Dictionary<string, (ASTType, TypeKind)>();

        private void ThrowsUnless<T>(Func<bool> action) where T: Exception, new()
        {
            if (!action()) throw new T();
        }

        public virtual void Check(BaseAST node) { }

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
            expr.RetType = expr.NodeType.isBoolOp()
                ?   TypeKind.Bool
                :   (TypeKind)Math.Max((int)expr.Lhs.RetType, (int)expr.Rhs.RetType);
            return true;
        });

        public void CheckAST(BlockExprAST expr) => ThrowsUnless<TypeCheckException>(() =>
        {
            this.Check(expr.Stmts);
            expr.RetType = expr.Stmts.Last().RetType;
            return true;
        });

        public void CheckAST(CallExprAST expr) => ThrowsUnless<TypeCheckException>(() =>
        {
            this.Check(expr.Arguments);
            if (!typeTbl.TryGetValue(expr.Callee, out var type)) return false;
            // check if the ident is function
            if (type.Item1 != ASTType.Prototype) return false;
            expr.RetType = type.Item2;
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
            expr.RetType = TypeKind.Int;
            return true;
        });

        public void CheckAST(IntExprAST expr) => ThrowsUnless<TypeCheckException>(() =>
        {
            expr.RetType = TypeKind.Float;
            return true;
        });

        public void CheckAST(FuncStmtAST stmt) => ThrowsUnless<TypeCheckException>(() =>
        {
            this.Check(stmt.Proto);
            this.Check(stmt.Body);
            typeTbl[stmt.Proto.Name] = (stmt.NodeType, stmt.Proto.FnRet);
            return true;
        });

        public void CheckAST(IfExprAST stmt) => ThrowsUnless<TypeCheckException>(() =>
        {
            var conds = stmt.Branches.Select(br => br.Cond).ToList();
            var bodies = stmt.Branches.Select(br => br.Body as BaseAST).ToList();
            this.Check(conds);
            this.Check(bodies);
            return true;
        });

        public void CheckAST(ProtoStmtAST stmt) => ThrowsUnless<TypeCheckException>(() =>
            typeTbl.TryAdd(stmt.Name, (stmt.NodeType, stmt.FnRet))
        );

        public void CheckAST(VariableExprAST expr) => ThrowsUnless<TypeCheckException>(() =>
        {
            if (!typeTbl.TryGetValue(expr.Name, out var type)) return false;
            // check if the ident is variable
            if (type.Item1 != ASTType.Variable) return false;
            expr.RetType = type.Item2;
            return true;
        });


    }
}
