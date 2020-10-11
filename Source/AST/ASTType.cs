namespace Small_lang.AST
{
    public enum ASTType
    {
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

        Variable,
        IntLit,
        FloatLit,
        BoolLit,
        Assign,
        If,
        Block,
        Nop,
        Read,
        Write,
        Repeat,
        For,
        Break,
        Continue
    }

    public enum TypeKind
    {
        Unit,
        Bool,
        Int, // i64
        Float, // double
    }
}
