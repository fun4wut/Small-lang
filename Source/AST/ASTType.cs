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
        Block,
    }

    public enum TypeKind
    {
        Unit,
        Bool,
        Int, // i64
        Float, // double
    }
}
