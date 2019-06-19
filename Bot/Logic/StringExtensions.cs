using System;
using System.Linq;
using System.Text;

namespace Bot
{
    public static class StringExtensions
    {
        public static string FetchSymbols(this string str, params int[] charPos)
        {
            return new string(charPos.Select(i => str[i]).ToArray());
        }

        public static string[] GetRows(this string str)
        {
            return str.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static int RowLen(this string str, int charPos)
        {
            var rowNumber = str.Where((c, i) => c == '\n' && i < charPos).Count() - 1;
            return str.GetRows()[rowNumber].Length + 2;
        }
        
        public static int PosUp(this string str, char c) => str.PosUp(str.IndexOf(c));
        public static int PosDown(this string str, char c) => str.PosDown(str.IndexOf(c));
        public static int PosLeft(this string str, char c) => str.PosLeft(str.IndexOf(c));
        public static int PosRight(this string str, char c) => str.PosRight(str.IndexOf(c));

        public static int PosUp(this string str, int charPos) => charPos - str.RowLen(charPos);
        public static int PosDown(this string str, int charPos) => charPos + str.RowLen(charPos);
        public static int PosLeft(this string str, int charPos) => charPos - 1;
        public static int PosRight(this string str, int charPos) => charPos + 1;

        public static string Random(params string[] strs)
        {
            return strs[new Random().Next(strs.Length)];
        }

        public static bool SameAs(this string str1, string str2)
        {
            return str1?.Trim().ToLower() == str2?.Trim().ToLower();
        }

        public static string ClearPos(this string map, int pos)
        {
            return map.Replace(pos, MapIcon.Empty);
        }

        public static string Replace(this string map, int pos, char toChar)
        {
            var newMap = new StringBuilder(map) {
                [pos] = toChar
            };
            return newMap.ToString();
        }
    }
}