using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PMachine
{
    public class Interpreter
    {
        private const int MaxSize = 3000;
        private PElm[] _stack = new PElm[MaxSize];
        private List<PIns> _instructions;
        private Dictionary<string, int> _labelDict = new Dictionary<string, int>();
        private int _pc = 0;
        private int _sp = -1;
        public Interpreter(string path)
        {
            var valid = File.ReadLines(path)
                .Where(s => s != string.Empty).ToList();
            // pre scan
            for (int i = 0; i < valid.Count; i++)
            {
                var ins = valid[i];
                if (ins.EndsWith(":"))
                {
                    _labelDict[ins.Substring(0, ins.Length-1)] = i;
                }
            }
            _instructions = valid.Select(line => new PIns(line)).ToList();
        }

        public void Run(bool verbose)
        {
            while (_pc >= 0)
            {
                if (verbose) Console.WriteLine($"--> ins: {_instructions[_pc]}");
                ExecSingle();
                if (verbose)
                {
                    for (var i = 0; i <= _sp; ++i)
                    {
                        Console.WriteLine($"location: {i}\t[{_stack[i].Value}]\ttype: {_stack[i].Type}");
                    }

                    Console.WriteLine($"\nSP = {_sp}\tPC = {_pc}\n****");
                }
                _pc++;
            }
        }
        
        private void DoBinOp(string op)
        {
            PElm.SameThen(_stack[_sp-1], _stack[_sp], (p1, p2) =>
            {
                var value = op switch
                {
                    "add" => p1.Value + p2.Value,
                    "sub" => p1.Value - p2.Value,
                    "mul" => p1.Value * p2.Value,
                    "div" => p1.Value / p2.Value,
                    "mod" => p1.Value % p2.Value,
                    "and" => p1.Value && p2.Value,
                    "or" => p1.Value || p2.Value,
                    "equ" => p1.Value == p2.Value,
                    "leq" => p1.Value <= p2.Value,
                    "les" => p1.Value < p2.Value,
                    "grt" => p1.Value > p2.Value,
                    "geq" => p1.Value >= p2.Value,
                    "neq" => p1.Value != p2.Value,
                    _ => throw new NotSupportedException()
                };
                _stack[--_sp] = new PElm(value);
            });
        }
        
        private void DoUnaryOp(string op)
        {
            var p = _stack[_sp];
            var value = op switch
            {
                "neg" => -p.Value,
                "not" => !p.Value,
                _ => throw new NotSupportedException()
            };
            _stack[_sp] = new PElm(value);
        }

        private void DoConversion(string toType)
        {
            var p = _stack[_sp];
            _stack[_sp] =  new PElm(toType switch
            {
                "i" => Convert.ToInt32(p.Value),
                "r" => Convert.ToDouble(p.Value),
                "b" => Convert.ToBoolean(p.Value),
                _ => throw new NotSupportedException()
            });
        }
        
        private void DoLit(PIns ins)
        {
            string s = ins.Hs2!;
            _stack[++_sp] =  new PElm(ins.Hs1 switch
            {
                "i" => int.Parse(s),
                "r" => double.Parse(s),
                "b" => s == "t",
                _ => throw new NotSupportedException()
            });
        }
        
        private void DoInput(PIns ins)
        {
            string? s;
            do
            {
                s = Console.ReadLine();
            } while (s == null);
            _stack[++_sp] =  new PElm(ins.Hs1 switch
            {
                "i" => int.Parse(s!),
                "r" => double.Parse(s!),
                "b" => bool.Parse(s!),
                _ => throw new NotSupportedException()
            });
        }


        public void ExecSingle()
        {
            var ins = _instructions[_pc];
            switch (ins.Op)
            {
                case "add":
                case "sub":
                case "mul":
                case "div":
                case "mod":
                case "and":
                case "or":
                case "equ":
                case "leq":
                case "les":
                case "grt":
                case "geq":
                case "neq":
                    DoBinOp(ins.Op);
                    break;
                case "neg":
                case "not":
                    DoUnaryOp(ins.Op);
                    break;
                case "conv":
                    DoConversion(ins.Hs2!);
                    break;
                case "ldc":
                    DoLit(ins);
                    break;
                case "dpl":
                    _stack[_sp+1] = new PElm(_stack[_sp].Value);
                    _sp++;
                    break;
                case string s when s.EndsWith(":"): // label, did nothing
                    break;
                case "ujp":
                    _pc = _labelDict[ins.Hs1!] - 1;
                    break;
                case "fjp":
                    if (_stack[_sp].Value == false)
                    {
                        _pc = _labelDict[ins.Hs1!] - 1;
                    }
                    _sp--;
                    break;
                case "lod":
                    _stack[++_sp] = _stack[int.Parse(ins.Hs3!)];
                    break;
                case "str":
                    _stack[int.Parse(ins.Hs3!)] = _stack[_sp--];
                    break;
                case "in":
                    DoInput(ins);
                    break;
                case "out":
                    Console.WriteLine($"print: {_stack[_sp--].Value}");
                    break;
                case "hlt":
                    _pc = -99;
                    break;
                default:
                    throw new NotImplementedException();
            }
            
        }
        
    }
    
    
}