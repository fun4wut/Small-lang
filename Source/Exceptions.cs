using System;
using System.Collections.Generic;
using System.Text;
using Small_lang.AST;

namespace Small_lang
{
    public class DupDeclException : TypeCheckException
    {
        public DupDeclException(string name)
        {
            _name = name;
        }

        public override string Message => $"variable {_name} duplicated";

        private readonly string _name;
    }

    public class UndefinedVarException : TypeCheckException
    {
        public UndefinedVarException(string name)
        {
            _name = name;
        }

        public override string Message => $"TypeCheckError:\tvariable {_name} undefined";

        private readonly string _name;
    }

    public class TypeNotAllowedException : TypeCheckException
    {
        public TypeNotAllowedException(TypeKind type, TypeKind? expected = null)
        {
            _type = type;
            _expected = expected;
        }

        public override string Message => $"TypeCheckError:\ttype {_type} is not allowed here, " +
                                          $"{(_expected != null ? $"expected is {_expected}" : "")}";

        private TypeKind _type;
        private TypeKind? _expected;
    }

    public class TypeCheckException : Exception { }

}
