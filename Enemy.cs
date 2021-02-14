using System;
using System.Collections.Generic;
using System.Text;

namespace VeganRPG
{
    class Enemy
    {
        // If enemy is boss, his quest and area will disappear right after winning fight
        bool boss;

        string name;

        int maxHealth;
        int health;
        Tuple<int, int> damage;
        int defense;

        int experience;
        Tuple<int, int> gold;
        List<Tuple<Item, int>> loot;

        public Enemy(string name, int health, Tuple<int, int> damage, int defense, 
            int experience, Tuple<int, int> gold, List<Tuple<Item, int>> loot, bool boss = false)
        {
            Boss = boss;

            Name = name;

            MaxHealth = health;
            Health = health;
            Damage = damage;
            Defense = defense;

            Experience = experience;
            Gold = gold;
            Loot = loot;
        }

        public int Experience { get => experience; set => experience = value; }
        public int Defense { get => defense; set => defense = value; }
        public Tuple<int, int> Damage { get => damage; set => damage = value; }
        public int Health { get => health; set => health = value; }
        public Tuple<int, int> Gold { get => gold; set => gold = value; }
        public List<Tuple<Item, int>> Loot { get => loot; set => loot = value; }
        public string Name { get => name; set => name = value; }
        public int MaxHealth { get => maxHealth; set => maxHealth = value; }
        public bool Boss { get => boss; set => boss = value; }
    }
}
