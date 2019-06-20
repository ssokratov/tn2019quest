using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot
{
    public static class SmileTranslator
    {
        private static readonly Dictionary<Char, string> ToSmileDict = new Dictionary<char, string> {
            [MapIcon.Toshik] = "\ud83d\udeb6\u200d\u2642\ufe0f",
            [MapIcon.Nastya] = "\ud83d\udc83\ud83c\udffc",
            //[MapIcon.Nastya] = "\ud83d\udc70\ud83c\udffc",
            //[MapIcon.Empty] = "\u2b1c\ufe0f",
            [MapIcon.Empty] = "\u25ab\ufe0f",
            [MapIcon.WallToshik] = "\u2b1b\ufe0f",
            [MapIcon.WallNastya] = "\u2b1b\ufe0f",
            //[MapIcon.Glasses] = "\ud83e\uddff",
            //[MapIcon.Glasses] = "\ud83d\udd05",
            [MapIcon.Glasses] = "\ud83e\uddd0",
            [MapIcon.StartDoors] = "\ud83d\udeaa",
            //[MapIcon.Fear] = "\ud83c\udf2b",
            [MapIcon.Fear] = "\u2716\ufe0f",
            //[MapIcon.Veil] = "\ud83d\udc52",
            [MapIcon.Veil] = "\ud83d\udc60",
            [MapIcon.Repa] = "\ud83e\udd35",
            [MapIcon.Sedosh] = "\ud83d\udc6e\u200d\u2642\ufe0f",
            //[MapIcon.JACOB] = "\ud83d\udc68\u200d\ud83d\udcbc",
            [MapIcon.Jacob] = "\ud83c\udf85",
            [MapIcon.Sokrat] = "\ud83d\udc68\u200d\ud83d\udcbb",
            [MapIcon.ZagsWorker] = "\ud83d\udc69\u200d\u2696\ufe0f",
            [MapIcon.Kolyan] = "\ud83d\ude47\u200d\u2642\ufe0f",
            [MapIcon.KolyanDacha] = "\ud83d\ude47\u200d\u2642\ufe0f",

            [MapIcon.Flame] = "\ud83d\udd25",
            [MapIcon.FireExtinguisher] = "\ud83e\uddef",
            [MapIcon.Boots] = "\ud83d\udc62",
            [MapIcon.SmallDoor] = "\ud83d\udeaa",
            [MapIcon.KolyanDachaDoor] = "\ud83d\udeaa",
            [MapIcon.KolyanDachaKey] = "\ud83d\udd11",

            [MapButtons.Left] = "\u2b05\ufe0f",
            [MapButtons.Up] = "\u2b06\ufe0f",
            [MapButtons.Right] = "\u27a1\ufe0f",
            [MapButtons.Down] = "\u2b07\ufe0f",
            [MapButtons.Journal] = "\ud83d\udcd6",
            [MapButtons.Inventory] = "\ud83d\udcbc",
            //[MapButtons.Inventory] = "\ud83d\udc5d",
        };

        private static readonly Dictionary<string, Char> FromSmileDict = ToSmileDict
            .Where(d => d.Key == MapButtons.Down
                        || d.Key == MapButtons.Up || d.Key == MapButtons.Left || d.Key == MapButtons.Right
                        || d.Key == MapButtons.Inventory || d.Key == MapButtons.Journal)
            .ToDictionary(m => m.Value, m => m.Key);

        public static string ToSmileAll(this string str)
        {
            return string.Join("", str.Select(c => c.ToSmile()));
        }

        public static string ToSmile(this char c)
        {
            if (ToSmileDict.ContainsKey(c)) {
                return ToSmileDict[c];
            }
            return c.ToString();
        }

        public static bool IsSmile(this string str)
        {
            return FromSmileDict.ContainsKey(str);
        }

        public static Char FromSmile(this string c)
        {
            if (FromSmileDict.ContainsKey(c)) {
                return FromSmileDict[c];
            }
            return c[0];
        }
    }
}