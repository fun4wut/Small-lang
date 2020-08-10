using NUnit.Framework;
using Kumiko_lang;
using Kumiko_lang.AST;
using System.Collections.Generic;
using System;
using Pidgin;

namespace Test
{
    public class ParserTests
    {

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void SingleLine()
        {
            var s = "a2a*(133+2.3);12;";
            var expected = new List<BaseAST>
            {
                new BinaryExprAST(
                    ASTType.Multiply,
                    new VariableExprAST("a2a"),
                    new BinaryExprAST(
                        ASTType.Add,
                        new IntExprAST(133),
                        new FloatExprAST(2.3)
                    )
                ),
                new IntExprAST(12)
            };
            Assert.AreEqual(expected, LangParser.ParseAll(s));
        }

        [Test]
        public void MultiLine()
        {
            var s = @"
a;;;


12;";
            var expected = new List<BaseAST>
            {
                new VariableExprAST("a"),
                new IntExprAST(12)
            };
            Assert.AreEqual(expected, LangParser.ParseAll(s));
        }

        [Test]
        public void ImmutVar()
        {
            var s = "let a12 = a*31;";
            var expected = new List<BaseAST>
            {
                new DeclStmtAST(
                    ASTType.Let,
                    "a12",
                    new BinaryExprAST(
                        ASTType.Multiply,
                        new VariableExprAST("a"),
                        new IntExprAST(31)
                    )
                )
            };
            Assert.AreEqual(expected, LangParser.ParseAll(s));
        }

        [Test]
        public void Prototype()
        {
            var s = "func xyz(a: Int, b: Float) -> Float;";
            var expected = new List<BaseAST>
            {
                new ProtoStmtAST(
                    "xyz",
                    new List<TypedArg>
                    {
                        new TypedArg ("a", TypeKind.Int),
                        new TypedArg ("b", TypeKind.Float)
                    },
                    TypeKind.Float
                )
            };
            Assert.AreEqual(expected, LangParser.ParseAll(s));
        }

        [Test]
        public void FuncDef()
        {
            var s = "func xyz(a: Int, b: Float) -> Int { 1; }";
            var proto = new ProtoStmtAST(
                "xyz",
                new List<TypedArg>
                {
                    new TypedArg ("a", TypeKind.Int),
                    new TypedArg ("b", TypeKind.Float)
                },
                TypeKind.Int
            );
            var expected = new List<BaseAST>
            {
                new FuncStmtAST(
                    proto,
                    new List<BaseAST>
                    {
                        new IntExprAST(1)
                    }.ToBlock()
                )
            };
            Assert.AreEqual(expected, LangParser.ParseAll(s));
        }

        [Test]
        public void Call()
        {
            var s = "ab(2.0, 3);";
            var expected = new List<BaseAST>
            {
                new CallExprAST(
                    "ab",
                    new List<BaseAST>
                    {
                        new FloatExprAST(2.0),
                        new IntExprAST(3)
                    }
                )
            };
            Assert.AreEqual(expected, LangParser.ParseAll(s));
        }

        [Test]
        public void Call_Throw()
        {
            var s = "1(2, 3);";
            Assert.Throws<Exception>(() => LangParser.ParseAll(s));
        }

        [Test]
        public void MutVar()
        {
            var s = "mut a = 3;";
            var expected = new List<BaseAST>
            {
                new DeclStmtAST(
                    ASTType.Mut,
                    "a",
                    new IntExprAST(3)
                )
            };
            Assert.AreEqual(expected, LangParser.ParseAll(s));
        }

        [Test]
        public void Assign()
        {
            var s = "a = 3;";
            var expected = new List<BaseAST>
            {
                new AssignStmtAST(
                    "a",
                    new IntExprAST(3)
                )
            };
            Assert.AreEqual(expected, LangParser.ParseAll(s));
        }

        [Test]
        public void Compare()
        {
            var s = "a >= 3;";
            var expected = new List<BaseAST>
            {
                new BinaryExprAST(
                    ASTType.GreaterEqual,
                    new VariableExprAST("a"),
                    new IntExprAST(3)
                )
            };
            Assert.AreEqual(expected, LangParser.ParseAll(s));
        }

        [Test]
        public void IfStmt()
        {
            var s = "if 2<3 { 2.3; a; } elif 3 > 4 { 5; } else { 7; }";
            var expected = new List<BaseAST>
            {
                new IfExprAST(
                    new List<Branch>
                    {
                        new Branch(
                            new BinaryExprAST(
                                ASTType.LessThan,
                                new IntExprAST(2),
                                new IntExprAST(3)
                            ),
                            new List<BaseAST>
                            {
                                new FloatExprAST(2.3),
                                new VariableExprAST("a")
                            }.ToBlock()
                        ),
                        new Branch(
                            new BinaryExprAST(
                                ASTType.GreaterThan,
                                new IntExprAST(3),
                                new IntExprAST(4)
                            ),
                            new List<BaseAST>
                            {
                                new IntExprAST(5)
                            }.ToBlock()
                        )
                    },
                    new ElseBranch(
                        new List<BaseAST>
                        {
                            new IntExprAST(7)
                        }.ToBlock()
                    )
                )
            };
            Assert.AreEqual(expected, LangParser.ParseAll(s));
        }

        [Test]
        public void IfExpr()
        {
            var s = "b = if 2<3 { 2; };";
            var expected = new List<BaseAST>
            {
                new AssignStmtAST(
                    "b",
                    new IfExprAST(
                        new List<Branch>
                        {
                            new Branch(
                                new BinaryExprAST(
                                    ASTType.LessThan,
                                    new IntExprAST(2),
                                    new IntExprAST(3)
                                ),
                                new List<BaseAST>
                                {
                                    new FloatExprAST(2.3),
                                    new VariableExprAST("a")
                                }.ToBlock()
                            )
                        }, null
                    )
                )
            };
            Assert.AreEqual(expected, LangParser.ParseAll(s));
        }

        [Test]
        public void Nested_If()
        {
            var s = "if true { if false { 4; } }";
            var expected = new List<BaseAST>
            {
                new IfExprAST(
                    new List<Branch>
                    {
                        new Branch(
                            new BoolExprAST(true),
                            new List<BaseAST>
                            {
                                new IfExprAST(
                                    new List<Branch>
                                    {
                                        new Branch(
                                            new BoolExprAST(false),
                                            new List<BaseAST>
                                            {
                                                new IntExprAST(4)
                                            }.ToBlock()
                                        )
                                    }, null
                                )
                            }.ToBlock()
                        )
                    }, null
                )
            };
            Assert.AreEqual(expected, LangParser.ParseAll(s));
        }

    }
}