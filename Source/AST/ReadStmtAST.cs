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
        public ReadStmtAST(string name, TypeKind retType)
        {
            this.Name = name;
            this.RetType = retType;
        }

        public string Name { get; }
        public override ASTType NodeType { get; protected set; } = ASTType.Read;

        protected internal override void Accept(ExprVisitor visitor) => visitor.VisitAST(this);
    }
}
