using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeganRPG
{
    class Area
    {
        readonly Random randomizer;

        string name;

        // Tuple with Enemy and chance to draw it from the list
        // Chance 10 means 1/10 chance
        // Chance 1 means 1/1 chance
        // Every area should contain one enemy with chance 1
        List<Tuple<Enemy, int>> enemies;

        public Area(string name, List<Tuple<Enemy, int>> enemies)
        {
            randomizer = new Random();

            Name = name;
            Enemies = enemies.OrderByDescending(x => x.Item2).ToList();
        }

        public Enemy LookForEnemy()
        {
            foreach (var enemy in Enemies)
            {
                if (randomizer.Next(enemy.Item2) == 0)
                {
                    return enemy.Item1;
                }
            }

            return Enemies.Last().Item1;
        }

        public string Name { get => name; set => name = value; }
        internal List<Tuple<Enemy, int>> Enemies { get => enemies; set => enemies = value; }
    }
}
