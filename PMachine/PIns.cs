using System;
using System.Collections.Generic;
using System.Linq;

namespace PMachine
{
    public class PIns
    {
        public PIns(string line)
        {
            _line = line;
            var fields = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Op = fields[0];
            Hs1 = GetOr(fields, 1);
            Hs2 = GetOr(fields, 2);
            Hs3 = GetOr(fields, 3);
        }
        public string Op { get; set; }
        public string? Hs1 { get; set; }
        public string? Hs2 { get; set; }
        public string? Hs3 { get; set; }
        private string _line;
        public override string ToString()
        {
            return _line;
        }

        private static string? GetOr(IReadOnlyList<string> arr, int idx) => idx < arr.Count ? arr[idx] : null;

    }

    public enum InsStat
    {
        Inc,
        Stay,
        Fin
    }
}