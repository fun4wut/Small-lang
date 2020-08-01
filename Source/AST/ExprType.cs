using System;
using System.Collections.Generic;
using System.Text;

namespace Kumiko_lang.AST
{
    public enum ExprType
    {
        AddExpr,
        SubtractExpr,
        MultiplyExpr,
        DivideExpr,
        LessThanExpr,
        CallExpr,
        VariableExpr,
        PrototypeExpr,
        FunctionExpr,
        NumberExpr
    }
}
