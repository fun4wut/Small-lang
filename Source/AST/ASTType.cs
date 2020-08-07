namespace Kumiko_lang.AST
{
    public enum ASTType
    {
        Prototype,
        Function,
        Add,
        Subtract,
        Multiply,
        Divide,
        LessThan,
        GreaterThan,
        LessEqual,
        GreatEqual,
        Equal,
        Call,
        Variable,
        Int,
        Float,
        Let,
        Mut,
        Assign,
        If,
    }

    public enum TypeKind
    {
        Int, // i64
        Float, // double
        Bool,
        Unit
    }
}
