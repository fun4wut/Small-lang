namespace Kumiko_lang.AST
{
    public enum ExprType
    {
        PrototypeExpr,
        FunctionExpr,
        AddExpr,
        SubtractExpr,
        MultiplyExpr,
        DivideExpr,
        LessThanExpr,
        CallExpr,
        VariableExpr,
        IntExpr,
        FloatExpr,
        AssignExpr,
    }

    public enum TypeEnum
    {
        Int, // i64
        Float, // double
        Bool,
        Unit
    }
}
