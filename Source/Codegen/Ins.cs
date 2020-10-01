﻿using System;
using Small_lang.AST;

namespace Small_lang.Codegen
{
    public static class Ins
    {
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
        public static string Geq(TypeKind ty) => $"geq {ty.S()}";
        public static string Leq(TypeKind ty) => $"leq {ty.S()}";
        public static string Neq(TypeKind ty) => $"neq {ty.S()}";
        public static string Ldo(TypeKind ty, int q) => $"ldo {ty.S()} {q}";
        public static string Conv(TypeKind before, TypeKind after) => $"conv {before.S()} {after.S()}";
        public static string Ldc(TypeKind ty, double q) => $"ldc {ty.S()} {q}";
        public static string Str(TypeKind ty, int p, int addr) => $"str {ty.S()} {p} {addr}";
        public static string Lod(TypeKind ty, int p, int addr) => $"lod {ty.S()} {p} {addr}";
        public static string Dpl(TypeKind ty) => $"dpl {ty.S()}";
        public static string In(TypeKind ty) => $"in {ty.S()}";
        public static string Out(TypeKind ty) => $"out {ty.S()}";

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