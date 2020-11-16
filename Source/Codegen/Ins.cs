using System;
using Small_lang.AST;

namespace Small_lang.Codegen
{
    public static class Ins
    {
        private static int LabelCnt = 0;
        public static string Hlt() => "hlt";
        public static string Pop() => "pop";
        public static string Add(TypeKind ty) => $"add {ty.S()}";
        public static string Sub(TypeKind ty) => $"sub {ty.S()}";
        public static string Mul(TypeKind ty) => $"mul {ty.S()}";
        public static string Mod() => "mod";
        public static string Not() => "not";
        public static string Div(TypeKind ty) => $"div {ty.S()}";
        public static string Neg(TypeKind ty) => $"neg {ty.S()}";
        public static string Equ(TypeKind ty) => $"equ {ty.S()}";
        public static string Les(TypeKind ty) => $"les {ty.S()}";
        public static string Grt(TypeKind ty) => $"grt {ty.S()}";
        public static string Geq(TypeKind ty) => $"geq {ty.S()}";
        public static string Leq(TypeKind ty) => $"leq {ty.S()}";
        public static string Neq(TypeKind ty) => $"neq {ty.S()}";
        public static string And() => "and";
        public static string Or() => "or";
        public static string Ldo(TypeKind ty, int q) => $"ldo {ty.S()} {q}";
        public static string Conv(TypeKind before, TypeKind after) => $"conv {before.S()} {after.S()}";
        public static string Ldc<T>(TypeKind ty, T q) => $"ldc {ty.S()} {q}";
        public static string Str(TypeKind ty, int p, int addr) => $"str {ty.S()} {p} {addr}";
        public static string Lod(TypeKind ty, int p, int addr) => $"lod {ty.S()} {p} {addr}";
        public static string Dpl(TypeKind ty) => $"dpl {ty.S()}";
        public static string In(TypeKind ty) => $"in {ty.S()}";
        public static string Out(TypeKind ty) => $"out {ty.S()}";

        public static string Fjp(string s) => $"fjp {s}";
        public static string Ujp(string s) => $"ujp {s}";
        public static string CreateLabel() => $"l{LabelCnt++}";
        public static string Label(string s) => $"{s}:";

        public static void CleanLabel() => LabelCnt = 0;
    }

    public static class TypeKindExt
    {
        public static string S(this TypeKind ty) => ty switch
        {
            TypeKind.Bool => "b",
            TypeKind.Float => "r",
            TypeKind.Int => "i",
            _ => throw new NotImplementedException()
        };
    }
    
}