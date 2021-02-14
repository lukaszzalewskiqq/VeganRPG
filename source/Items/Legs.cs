using System;
using System.Collections.Generic;
using System.Text;

namespace VeganRPG
{
    class Legs : Item
    {
        public Legs() : base()
        {

        }

        public Legs(int level, string name, int Health, int defense, bool isBase = false)
        {
            this.Level = level;
            this.Name = name;
            this.Health = Health;
            this.Defense = defense;
            this.IsBase = isBase;
        }
    }
}
