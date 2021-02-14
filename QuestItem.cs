using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeganRPG
{
    class QuestItem : Item
    {
        int count;

        public QuestItem(string name)
        {
            Name = name;

            count = 1;
        }

        public override void Info(bool newLine = true)
        {
            if (!newLine)
            {
                Util.Write(Name, ConsoleColor.Red);
            }
            else
            {
                Util.WriteLine(Name, ConsoleColor.Red);
            }
        }

        public override int Value()
        {
            return 0;
        }

        public int Count { get => count; set => count = value; }
    }
}
