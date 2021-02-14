using System;
using System.Collections.Generic;
using System.Text;

namespace VeganRPG
{
    class Boots : Item
    {
        public Boots() : base()
        {

        }

        public Boots(int level, string name, int Health, int defense, bool isBase = false)
        {
            this.Level = level;
            this.Name = name;
            this.Health = Health;
            this.Defense = defense;
            this.IsBase = isBase;
        }
    }
}
