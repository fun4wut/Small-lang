using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kumiko_lang.TypeCheck;
namespace Kumiko_lang.AST
{
    [ToString]
    [Equals(DoNotAddEqualityOperators = true)]
    public sealed class BlockExprAST : BaseAST
    {
        public BlockExprAST(IEnumerable<BaseAST> stmts)
        {
            Stmts = stmts.ToList();
        }

        public List<BaseAST> Stmts { get; }
        public override ASTType NodeType { get; protected set; } = ASTType.Block;

        protected internal override BaseAST? Accept(ExprVisitor visitor) => visitor.VisitAST(this);

        protected internal override void CheckWith(TypeChecker checker) => checker.CheckAST(this);
    }

    public static class ListExt
    {
        public static BlockExprAST ToBlock(this List<BaseAST> lst) => new BlockExprAST(lst);
    }
}
