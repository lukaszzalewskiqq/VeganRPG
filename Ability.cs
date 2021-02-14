using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace VeganRPG
{
    class Ability
    {
        string name;
        string description;

        int level;
        int maxLevel;

        // Fight uses variable turns to manipulate in-place how many turns are left till ability effects stop
        // TurnsConst is variable that's needed to restore old value after fight
        int turns;
        int turnsConst;

        int cost;

        AbilityEffects abilityEffects;

        public Ability(string name, string description, int level, int maxLevel, int turns, int cost, AbilityEffects abilityEffects)
        {
            Name = name;
            Description = description;

            Level = level;
            MaxLevel = maxLevel;
            Turns = turns;
            TurnsConst = turns;
            Cost = cost;

            AbilityEffects = abilityEffects;
        }

        public void Use(ref AbilityEffects effectsCombined)
        {
            var properties = effectsCombined.GetType().GetProperties();

            foreach (var prop in properties)
            {
                prop.SetValue(effectsCombined, Convert.ToDouble(prop.GetValue(effectsCombined))
                    + Convert.ToDouble(prop.GetValue(AbilityEffects)));
            }
        }

        public void Stop(ref AbilityEffects effectsCombined)
        {
            var properties = effectsCombined.GetType().GetProperties();

            foreach (var prop in properties)
            {
                prop.SetValue(effectsCombined, Convert.ToDouble(prop.GetValue(effectsCombined))
                    - Convert.ToDouble(prop.GetValue(AbilityEffects)));
            }
        }

        public virtual void Info()
        {
            string info = "@11|" + Cost + " " + Name + " @15|- ";

            Util.WriteColorString(info);
  
            Util.WriteColorString(description);
        }

        public int Level { get => level; set => level = value; }
        public int Turns { get => turns; set => turns = value; }
        public int Cost { get => cost; set => cost = value; }
        public string Name { get => name; set => name = value; }
        public string Description { get => description; set => description = value; }
        public int MaxLevel { get => maxLevel; set => maxLevel = value; }
        public int TurnsConst { get => turnsConst; set => turnsConst = value; }
        internal AbilityEffects AbilityEffects { get => abilityEffects; set => abilityEffects = value; }
    }
}
