using System;
using System.Collections.Generic;
using System.Text;
namespace Kumiko_lang.AST
{
    public static class ASTExtensions
    {
        public static void Compile(this List<BaseAST> exprASTs, ExprVisitor visitor) =>
            exprASTs.ForEach(e => visitor.Visit(e));

        public static void Compile(this BaseAST exprAST, ExprVisitor visitor) => visitor.Visit(exprAST);

        public static int ASTValue(this ASTType ty) => ty switch
        {
            ASTType.Prototype => -2,
            ASTType.Function => -1,
            _ => 0
        };

        public static bool isBoolOp(this ASTType ty) => (int)ty >= (int)ASTType.LessThan && (int)ty <= (int)ASTType.Equal;
    }
}
