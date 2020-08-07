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
        GreaterThanExpr,
        LessEqualExpr,
        GreatEqualExpr,
        EqualExpr,
        CallExpr,
        VariableExpr,
        IntExpr,
        FloatExpr,
        LetExpr,
        MutExpr,
        AssignExpr,
        IfExpr,
    }

    public enum TypeEnum
    {
        Int, // i64
        Float, // double
        Bool,
        Unit
    }
}
