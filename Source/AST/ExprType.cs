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
        IntExpr,
        FloatExpr,
        AssignExpr,
    }

    public enum TypeEnum
    {
        Int, // i64
        Float, // double
        Bool
    }
}
