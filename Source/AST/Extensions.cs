using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Small_lang.AST
{
    public static class ASTExtensions
    {
        public static void Compile(this List<BaseAST> exprASTs, ASTVisitor visitor) =>
            exprASTs.ForEach(e => visitor.Visit(e));

        public static void Compile(this BaseAST exprAST, ASTVisitor visitor) => visitor.Visit(exprAST);
        
        public static bool IsBoolOp(this ASTType ty) => (int)ty >= (int)ASTType.LessThan && (int)ty <= (int)ASTType.Equal;
    }
}
