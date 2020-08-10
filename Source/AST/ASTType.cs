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
        GreaterEqual,
        Equal,
        Call,
        Variable,
        IntIdent,
        FloatIdent,
        BoolIdent,
        Let,
        Mut,
        Assign,
        If,
        Block,
        Nop,
    }

    public enum TypeKind
    {
        Unit,
        Bool,
        Int, // i64
        Float, // double
    }
}
