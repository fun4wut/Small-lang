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
    }
}