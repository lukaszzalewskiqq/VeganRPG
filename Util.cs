using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 2 DARKGREEN - Stats, Areas
// 4 DARKRED - Save, Load, Experience, Level
// 5 DARKMAGENTA - Defense
// 6 DARKYELLOW - Gold
// 8 DARKGRAY - Items
// 9 BLUE - NPC, Enemy
// 10 GREEN - Cities
// 11 CYAN - Abilities, AP
// 12 RED - Quest, Health
// 13 MAGENTA - Damage
// 14 YELLOW - Consumables
// 15 WHITE - Text

namespace VeganRPG
{
    class Util
    { 
        public static void Write(string text, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;

            Console.Write(text);

            Console.ResetColor();
        }

        public static void WriteLine(string text, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;

            Console.WriteLine(text);

            Console.ResetColor();
        }

        public static void WriteMulticolor(string text)
        {
            ConsoleColor color;
            for (int i = 0; i < text.Length; ++i)
            {
                if ((i % 16) != 0)
                {
                    color = (ConsoleColor)(i % 16);
                }
                else
                {
                    color = (ConsoleColor)((i + 1) % 16);
                }

                Util.Write(text[i].ToString(), color);
            }
        }

        public static void WriteColorString(string text, bool newLine = false)
        {
            List<Tuple<string, int>> coloredStringList = new List<Tuple<string, int>>();
            List<string> stringList;

            if (!text.Contains('@'))
            {
                Util.Write(text);
            }
            else
            {
                stringList = text.Split('@').ToList();
                stringList.RemoveAll(x => x == "");

                string colorString;
                string coloredString;

                foreach (var str in stringList)
                {
                    colorString = str.Substring(0, str.IndexOf('|'));
                    coloredString = str.Substring(str.IndexOf('|') + 1, str.Length - colorString.Length - 1);

                    coloredStringList.Add(new Tuple<string, int>(coloredString, Convert.ToInt32(colorString)));
                }

                foreach (var str in coloredStringList)
                {
                    if (str.Item2 == 15)
                    {
                        Util.Write(str.Item1);
                    }
                    else
                    {
                        Util.Write(str.Item1, (ConsoleColor)str.Item2);
                    }
                }
            }

            if (newLine)
            {
                Util.Write("\n");
            }
        }

        public static int NumpadKeyToInt(ConsoleKeyInfo key)
        {
            return key.Key switch
            {
                ConsoleKey.NumPad0 => 0,
                ConsoleKey.NumPad1 => 1,
                ConsoleKey.NumPad2 => 2,
                ConsoleKey.NumPad3 => 3,
                ConsoleKey.NumPad4 => 4,
                ConsoleKey.NumPad5 => 5,
                ConsoleKey.NumPad6 => 6,
                ConsoleKey.NumPad7 => 7,
                ConsoleKey.NumPad8 => 8,
                ConsoleKey.NumPad9 => 9,
                _ => -1
            };
        }
    }
}
