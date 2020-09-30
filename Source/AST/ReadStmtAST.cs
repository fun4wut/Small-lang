using Small_lang.TypeCheck;
using System;
using System.Collections.Generic;
using System.Text;

namespace Small_lang.AST
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public class ReadStmtAST : BaseAST
    {
        public ReadStmtAST(string name, TypeKind varType)
        {
            this.Name = name;
            this.VarType = varType;
        }

        public string Name { get; }
        public TypeKind VarType { get; }
        public override ASTType NodeType { get; protected set; } = ASTType.Read;

        protected internal override void Accept(ASTVisitor visitor) => visitor.VisitAST(this);
    }
}
