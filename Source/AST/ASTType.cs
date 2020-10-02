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
        Neg,
        Not,
        
        // Bool op start
        LessThan,
        GreaterThan,
        LessEqual,
        GreaterEqual,
        NotEqual,
        And,
        Or,
        Equal,
        // Bool op end

        Call,
        Variable,
        IntLit,
        FloatLit,
        BoolLit,
        Let,
        Mut,
        Assign,
        If,
        Block,
        Nop,
        Read,
        Write,
        Repeat,
        For
    }

    public enum TypeKind
    {
        Unit,
        Bool,
        Int, // i64
        Float, // double
    }
}
