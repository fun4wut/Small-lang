using System;

namespace PMachine
{
    public class PElm
    {
        public PElm(dynamic value)
        {
            Value = value;
            Type = value switch
            {
                int i => TypeKind.Int,
                double f => TypeKind.Float,
                bool b => TypeKind.Bool,
                _ => throw new NotImplementedException()
            };
        }

        public dynamic Value { get; set; }
        public TypeKind Type { get; set;}


        public static void SameThen(PElm p1, PElm p2, Action<PElm, PElm> action)
        {
            if (p1.Type == p2.Type) action(p1, p2);
        }
    }

    public enum TypeKind
    {
        Int,
        Float,
        Bool
    }
}