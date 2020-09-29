namespace Small_lang.AST
{
    public enum ASTType
    {
        Prototype,
        Function,
        Add,
        Subtract,
        Multiply,
        Divide,
        Modulo,

        // Bool op start
        LessThan,
        GreaterThan,
        LessEqual,
        GreaterEqual,
        NotEqual,
        Equal,
        // Bool op end

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
        Read,
        Write,
        Repeat
    }

    public enum TypeKind
    {
        Unit,
        Bool,
        Int, // i64
        Float, // double
    }
}
