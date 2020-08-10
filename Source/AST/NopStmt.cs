using Kumiko_lang.TypeCheck;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kumiko_lang.AST
{
    public sealed class NopStmt : BaseAST
    {
        public override ASTType NodeType { get; protected set; } = ASTType.Nop;

        protected internal override BaseAST? Accept(ExprVisitor visitor) => this;

        protected internal override void CheckWith(TypeChecker cheker) { }
    }
}
