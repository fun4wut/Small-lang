using System;
using Small_lang.AST;

namespace Small_lang.Codegen
{
    partial class Codegenerator : ASTVisitor
    {
        
        protected internal override void VisitAST(IntExprAST node)
        {
            GenCode.Add(Ins.Ldc(node.RetType, node.Value));
        }

        protected internal override void VisitAST(FloatExprAST node)
        {
            GenCode.Add(Ins.Ldc(TypeKind.Float, node.Value));
        }

        protected internal override void VisitAST(VariableExprAST node)
        {
            var (_, addr) = GetId(node.Name);
            GenCode.Add(Ins.Lod(node.RetType, 0, addr));
        }
        protected internal override void VisitAST(BinaryExprAST node)
        {
            // for convenience, convert both of the LHS and RHS type explicitly 
            var ty = (TypeKind) Math.Max((int) node.Lhs.RetType, (int) node.Rhs.RetType);
            
            this.Visit(node.Lhs);
            GenCode.Add(Ins.Conv(node.Lhs.RetType,ty));
            this.Visit(node.Rhs);
            GenCode.Add(Ins.Conv(node.Rhs.RetType, ty));

            var code = node.NodeType switch
            {
                ASTType.Add => Ins.Add(ty),
                ASTType.Subtract => Ins.Sub(ty),
                ASTType.Multiply => Ins.Mul(ty),
                ASTType.Divide => Ins.Div(ty),
                ASTType.Modulo => Ins.Mod(),
                ASTType.Equal => Ins.Equ(ty),
                ASTType.NotEqual => Ins.Neq(ty),
                ASTType.LessThan => Ins.Les(ty),
                ASTType.LessEqual => Ins.Leq(ty),
                ASTType.GreaterThan => Ins.Grt(ty),
                ASTType.GreaterEqual => Ins.Geq(ty),
                ASTType.And => Ins.And(),
                ASTType.Or => Ins.Or(),
                _ => throw new NotImplementedException()
            };
            GenCode.Add(code);
        }

        protected internal override void VisitAST(UnaryExprAST node)
        {
            this.Visit(node.Hs);
            var code = node.NodeType switch
            {
                ASTType.Not => Ins.Not(),
                ASTType.Neg => Ins.Neg(node.RetType),
                _ => throw new NotImplementedException()
            };
            GenCode.Add(code);
        }

        protected internal override void VisitAST(BlockExprAST node)
        {
            this.Visit(node.Stmts);
        }

        protected internal override void VisitAST(BoolExprAST node)
        {
            GenCode.Add(Ins.Ldc(TypeKind.Bool, node.Value ? "t" : "f"));
        }
    }
}