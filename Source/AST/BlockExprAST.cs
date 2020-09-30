using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Small_lang.TypeCheck;
namespace Small_lang.AST
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

        protected internal override void Accept(ASTVisitor visitor) => visitor.VisitAST(this);

    }

    public static class ListExt
    {
        public static BlockExprAST ToBlock(this List<BaseAST> lst) => new BlockExprAST(lst);
    }
}
