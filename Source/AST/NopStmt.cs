using Small_lang.TypeCheck;
using System;
using System.Collections.Generic;
using System.Text;

namespace Small_lang.AST
{
    public sealed class NopStmt : BaseAST
    {
        public override ASTType NodeType { get; protected set; } = ASTType.Nop;

        protected internal override void Accept(ExprVisitor visitor) {}
    }
}
