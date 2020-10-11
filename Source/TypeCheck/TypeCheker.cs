using System;
using System.Collections.Generic;
using System.Linq;
using Small_lang.AST;

namespace Small_lang.TypeCheck
{
    public class TypeChecker : ASTVisitor
    {
        public TypeChecker() { }

        private readonly Dictionary<string, (ASTType, TypeKind)> _typeTbl = new Dictionary<string, (ASTType, TypeKind)>();
        
        public override void Clear()
        {
            _typeTbl.Clear();
        }
        
        protected internal override void VisitAST(AssignStmtAST node)
        {

            this.Visit(node.Value);

            // Unit type is not allowed
            if (!node.Value.IsExpr) throw new TypeNotAllowedException(node.Value.RetType);

            if (_typeTbl.TryGetValue(node.Name, out var type))
            {
                if (type.Item2 != node.Value.RetType)
                {
                    // throw error if type mismatch
                    throw new TypeNotAllowedException(node.Value.RetType, type.Item2);
                }
            } 
            else
            {
                _typeTbl.Add(node.Name, (node.NodeType, node.Value.RetType));
            }
        }


        protected internal override void VisitAST(BinaryExprAST node)
        {
            this.Visit(node.Lhs);
            this.Visit(node.Rhs);
            // only expr can involve in binary operation
            if (!node.Lhs.IsExpr || !node.Rhs.IsExpr) throw new TypeNotAllowedException(node.Lhs.RetType);

            var widenType = (TypeKind)Math.Max((int)node.Lhs.RetType, (int)node.Rhs.RetType);
            var shrinkType = (TypeKind)Math.Min((int)node.Lhs.RetType, (int)node.Rhs.RetType);
            
            // bool mix number is not allowed
            if (widenType != TypeKind.Bool && shrinkType == TypeKind.Bool)
            {
                throw new TypeNotAllowedException(node.Lhs.RetType, TypeKind.Bool);
            }

            // and or need 2 bools
            if ((node.NodeType == ASTType.And || node.NodeType == ASTType.Or) && shrinkType != TypeKind.Bool)
            {
                throw new TypeNotAllowedException(node.Lhs.RetType, TypeKind.Bool);
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
                    throw new TypeNotAllowedException(node.Lhs.RetType, TypeKind.Int);
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
        

        protected internal override void VisitAST(FloatExprAST node)
        {
            node.RetType = TypeKind.Float;
        }

        protected internal override void VisitAST(IntExprAST node)
        {
            node.RetType = TypeKind.Int;
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
                if (cond.RetType != TypeKind.Bool)
                {
                    throw new TypeNotAllowedException(cond.RetType, TypeKind.Bool);
                }
            }
            this.Visit(bodies);
            this.Visit(@else?.Body);
        }

        protected internal override void VisitAST(VariableExprAST node)
        {
            // check exists
            if (!_typeTbl.TryGetValue(node.Name, out var type)) throw new UndefinedVarException(node.Name);
            node.RetType = type.Item2;
        }

        protected internal override void VisitAST(ReadStmtAST node)
        {

            if (_typeTbl.TryGetValue(node.Name, out var type))
            {
                if (type.Item2 != node.VarType)
                {
                    // throw error if type mismatch
                    throw new TypeNotAllowedException(node.VarType, type.Item2);
                }
            }
            else
            {
                _typeTbl.Add(node.Name, (node.NodeType, node.VarType));
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
            if (cond.RetType != TypeKind.Bool) throw new TypeNotAllowedException(cond.RetType, TypeKind.Bool);
            this.Visit(body);
        }
        
        protected internal override void VisitAST(RepeatStmtAST node)
        {
            var cond = node.InfLoop.Cond;
            var body = node.InfLoop.Body;

            this.Visit(cond);
            if (cond.RetType != TypeKind.Bool) throw new TypeNotAllowedException(cond.RetType, TypeKind.Bool);
            this.Visit(body);
        }

        protected internal override void VisitAST(UnaryExprAST node)
        {
            this.Visit(node.Hs);
            if (!node.Hs.IsExpr) throw new TypeCheckException();
            node.RetType = node.NodeType switch
            {
                ASTType.Not when node.Hs.RetType != TypeKind.Bool 
                    => throw new TypeNotAllowedException(node.Hs.RetType, TypeKind.Float),
                ASTType.Neg when node.Hs.RetType == TypeKind.Bool 
                    => throw new TypeNotAllowedException(node.Hs.RetType, TypeKind.Bool),
                _ => node.Hs.RetType
            };
        }
    }
}
