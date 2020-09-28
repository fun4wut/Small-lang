using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Small_lang.TypeCheck;
namespace Small_lang.AST
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public sealed class FuncStmtAST : BaseAST
    {
        public FuncStmtAST(ProtoStmtAST proto, BlockExprAST body)
        {
            Proto = proto;
            Body = body;
        }

        public ProtoStmtAST Proto { get; private set; }
        public BlockExprAST Body { get; private set; }
        public override ASTType NodeType { get; protected set; } = ASTType.Function;

        protected internal override BaseAST? Accept(ExprVisitor visitor) => visitor.VisitAST(this);
        protected internal override void CheckWith(TypeChecker checker) => checker.CheckAST(this);
    }
}
