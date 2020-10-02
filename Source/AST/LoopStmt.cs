using Small_lang.TypeCheck;
using System;
using System.Collections.Generic;
using System.Text;

namespace Small_lang.AST
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public sealed class LoopStmt : BaseAST
    {
        public LoopStmt(Branch infLoop, BaseAST? preRun = null, BaseAST? postRun = null)
        {
            InfLoop = infLoop;
            PreRun = preRun;
            PostRun = postRun;
            NodeType = preRun == null ? ASTType.Repeat : ASTType.For;
        }

        public BaseAST? PreRun { get; }
        public BaseAST? PostRun { get; } 
        public Branch InfLoop { get; }
        public override ASTType NodeType { get; protected set; }

        protected internal override void Accept(ASTVisitor visitor) => visitor.VisitAST(this);
    }
}
