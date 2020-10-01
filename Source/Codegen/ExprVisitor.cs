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
            var ty = node.RetType;
            // for convenience, convert both of the LHS and RHS type explicitly 
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
                ASTType.Neg => Ins.Neg(node.RetType)
            };
            GenCode.Add(code);
        }
    }
}