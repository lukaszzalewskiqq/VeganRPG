using System;
using System.Collections.Generic;
using System.Text;

namespace VeganRPG
{
    class Consumable : Item
    {
        int count;
        int ap;

        public Consumable() : base()
        {
            Count = 1;
        }

        public Consumable(int level, string name, int Health, int ap)
        {
            this.Level = level;
            this.Name = name;
            this.Health = Health;
            this.Ap = ap;
            this.Count = 1;
        }

        public override int Value()
        {
            return (Health / 10) + Ap;
        }

        public override void Info(bool newLine = true)
        {
            Util.Write(Name + " ", ConsoleColor.Yellow);
            Util.Write("Level " + Level + " ", ConsoleColor.DarkRed);
            Util.Write("Health " + Health + " ", ConsoleColor.Red);

            if (!newLine)
            {
                Util.Write("Ability points " + Ap, ConsoleColor.Cyan);
            }
            else
            {
                Util.WriteLine("Ability points " + Ap, ConsoleColor.Cyan);
            }
        }

        public int Count { get => count; set => count = value; }
        public int Ap { get => ap; set => ap = value; }
    }
}
