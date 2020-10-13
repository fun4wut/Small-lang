using NUnit.Framework;
using Small_lang;
using Small_lang.AST;
using System.Collections.Generic;
using System;
using System.Linq;
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
            var s = "a2a*(133+2.3);12";
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
a;


12";
            var expected = new List<BaseAST>
            {
                new VariableExprAST("a"),
                new IntExprAST(12)
            };
            Assert.AreEqual(expected, LangParser.ParseAll(s));
        }
        


        [Test]
        public void Assign()
        {
            var s = "a := 3";
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
            var s = "a >= 3";
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
        public void AndOr()
        {
            var s = "a >= 2 && true || false";
            var expected = new List<BaseAST>
            {
                new BinaryExprAST(
                    ASTType.Or,
                    new BinaryExprAST(
                        ASTType.And,
                        new BinaryExprAST(
                            ASTType.GreaterEqual,
                            new VariableExprAST("a"),
                            new IntExprAST(2)
                        ),
                        new BoolExprAST(true)
                    ),
                    new BoolExprAST(false)
                )
            };
            Assert.AreEqual(expected, LangParser.ParseAll(s));
        }

        [Test]
        public void IfElse()
        {
            var s = "if 2<3 then 2.3 else 7 end";
            var expected = new List<BaseAST>
            {
                new IfStmtAST(
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
        public void OnlyIf()
        {
            var s = "if 2<3 then 2 end";
            var expected = new List<BaseAST>
            {
                new IfStmtAST(
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
            };
            Assert.AreEqual(expected, LangParser.ParseAll(s));
        }

        [Test]
        public void Nested_If()
        {
            var s = @"
if true then 
    if false then 
        write 4
    else
        write 5
    end
end";
            var expected = new List<BaseAST>
            {
                new IfStmtAST(
                    new List<Branch>
                    {
                        new Branch(
                            new BoolExprAST(true),
                            new List<BaseAST>
                            {
                                new IfStmtAST(
                                    new List<Branch>
                                    {
                                        new Branch(
                                            new BoolExprAST(false),
                                            new List<BaseAST>
                                            {
                                                new WriteStmtAST(new IntExprAST(4))
                                            }.ToBlock()
                                        )
                                    },
                                    new ElseBranch(
                                        new List<BaseAST>
                                        {
                                            new WriteStmtAST(new IntExprAST(5))
                                        }.ToBlock()
                                    )
                                )
                            }.ToBlock()
                        )
                    }, null
                )
            };
            Assert.AreEqual(expected, LangParser.ParseAll(s));
        }

        [Test]
        public void ReadStmt()
        {
            var s = "read a";
            var expected = new List<BaseAST> { new ReadStmtAST("a", TypeKind.Int) };
            Assert.AreEqual(expected, LangParser.ParseAll(s));
        }

        [Test]
        public void WriteStmt()
        {
            var s = "write a";
            var expected = new List<BaseAST> { new WriteStmtAST(new VariableExprAST("a")) };
            Assert.AreEqual(expected, LangParser.ParseAll(s));
        }

        [Test]
        public void RepeatStmt()
        {
            var s = @"
repeat 
/*
Comment
*/
    a := 3; // 233
    // 456
    4
until true";
            var expected = new List<BaseAST>
            {
                new RepeatStmtAST(new Branch(
                    new BoolExprAST(true),
                    new BlockExprAST(new List<BaseAST>
                    {
                        new AssignStmtAST("a", new IntExprAST(3)),
                        new IntExprAST(4)
                    }
                    ))
                )
            };
            Assert.AreEqual(expected, LangParser.ParseAll(s));
        }
        
        [Test]
        public void ForStmt()
        {
            var s = @"
for a := 2; a < 10; a := a + 1
begin
    write a
end";
            var expected = new List<BaseAST>
            {
                new ForStmtAST(new Branch(
                    new BinaryExprAST(ASTType.LessThan, new VariableExprAST("a"), new IntExprAST(10)), 
                    new BlockExprAST(new List<BaseAST>
                        {
                            new WriteStmtAST(new VariableExprAST("a"))
                        }
                    )),
                    new AssignStmtAST("a", new IntExprAST(2)),
                    new AssignStmtAST("a", new BinaryExprAST(
                        ASTType.Add,
                        new VariableExprAST("a"),
                        new IntExprAST(1)
                    ))
                )
            };
            Assert.AreEqual(expected, LangParser.ParseAll(s));
        }

        [Test]
        public void AtLeastOneStmt()
        {
            var s = @"";
            Assert.Throws<ParseException>(() => LangParser.ParseAll(s));
        }
        
    }
}