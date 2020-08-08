using System;
using System.Collections.Generic;
using System.Linq;
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
        public static bool IsBoolOp(this ASTType ty) => (int)ty >= (int)ASTType.LessThan && (int)ty <= (int)ASTType.Equal;
        public static BaseAST MakeMain(this IEnumerable<BaseAST> stmts) =>
            new FuncStmtAST(
                new ProtoStmtAST(
                    "main",
                    new List<TypedArg>(),
                    TypeKind.Int
                ),
                stmts.Append(new IntExprAST(0)).ToList().ToBlock()
            );
    }
}