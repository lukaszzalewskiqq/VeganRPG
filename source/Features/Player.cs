using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace VeganRPG
{
    public class Player
    {
        int experienceMultipler;

        readonly int healthPerLevel;
        int apPerLevel;

        List<Quest> questsDone;

        int experience;
        int level;
        int gold;

        int ap;

        int maxHealth;
        int health;
        Tuple<int, int> damage;
        int defense;

        List<Item> itemStash;
        List<Consumable> consumables;

        List<QuestItem> questItems;

        Helmet helmet;
        Armor armor;
        Legs legs;
        Boots boots;
        Weapon weapon;

        internal Player(List<Item> items)
        {
            ExperienceMultipler = 10;

            QuestsDone = new List<Quest>();

            healthPerLevel = 20;
            ApPerLevel = 5;

            Experience = 0;
            Level = 0;
            Gold = 0;

            Ap = 0;

            MaxHealth = healthPerLevel;
            Health = healthPerLevel;
            damage = new Tuple<int, int>(0, 0);
            defense = 0;

            ItemStash = new List<Item>();
            Consumables = new List<Consumable>();

            QuestItems = new List<QuestItem>();

            Weapon = (Weapon) items.Find(x => x.Name == "Hands");
            Helmet = (Helmet) items.Find(x => x.Name == "Head");
            Armor = (Armor) items.Find(x => x.Name == "Torso");
            Legs = (Legs) items.Find(x => x.Name == "Legs");
            Boots = (Boots) items.Find(x => x.Name == "Feet");

            CalculateStats();
        }

        public bool CalculateStats()
        {
            bool levelUp = CalculateLvl();

            if (levelUp)
            {
                if (ap + apPerLevel > level * apPerLevel)
                {
                    ap = level * apPerLevel;
                }
                else
                {
                    ap += apPerLevel;
                }
            }

            maxHealth = ((level + 1) * healthPerLevel) + Helmet.Health + Armor.Health + Legs.Health + Boots.Health;
            damage = Weapon.Damage;
            defense = Helmet.Defense + Armor.Defense + Legs.Defense + Boots.Defense;

            return levelUp;
        }

        bool CalculateLvl()
        {
            if (experience >= ExperienceToLevel(Level))
            {
                level += 1;
                return true;
            }
            else if (experience < ExperienceToLevel(Level - 1))
            {
                level -= 1;
                return true;
            }

            return false;
        }

        public void ShowStatistics()
        {
            Console.Clear();

            Util.Write("Helmet: ");
            Helmet.Info();

            Util.Write("Armor: ");
            Armor.Info();

            Util.Write("Legs: ");
            Legs.Info();

            Util.Write("Boots: ");
            Boots.Info();

            Util.Write("Weapon: ");
            Weapon.Info();

            Util.Write("\nExperience", ConsoleColor.DarkRed);
            Util.Write(": ");
            Util.Write(Experience + " ", ConsoleColor.DarkRed);
            Util.Write("/ ");
            Util.WriteLine(ExperienceToLevel(level) + "", ConsoleColor.DarkRed);

            Util.Write("Level", ConsoleColor.DarkRed);
            Util.Write(": ");
            Util.WriteLine(Level + "\n", ConsoleColor.DarkRed);

            Util.Write("Gold", ConsoleColor.DarkYellow);
            Util.Write(": ");
            Util.WriteLine(Gold + "\n", ConsoleColor.DarkYellow);

            Util.Write("Health", ConsoleColor.Red);
            Util.Write(": ");
            Util.Write(Health + "", ConsoleColor.Red);
            Util.Write(" / ");
            Util.WriteLine(MaxHealth + "", ConsoleColor.Red);

            Util.Write("Damage", ConsoleColor.Magenta);
            Util.Write(": ");
            Util.Write(Damage.Item1 + "", ConsoleColor.Magenta);
            Util.Write(" - ");
            Util.WriteLine(Damage.Item2 + "", ConsoleColor.Magenta);

            Util.Write("Defense", ConsoleColor.DarkMagenta);
            Util.Write(": ");
            Util.WriteLine(Defense + "\n", ConsoleColor.DarkMagenta);

            Util.Write("Ability points", ConsoleColor.Cyan);
            Util.Write(": ");
            Util.Write(Ap + "", ConsoleColor.Cyan);
            Util.Write(" / ");
            Util.WriteLine((Level * ApPerLevel) + "\n", ConsoleColor.Cyan);

            Console.ReadKey();
        }

        public void ShowItemStash()
        {
            Console.Clear();

            OrderItemStashList();

            Util.Write("Item ", ConsoleColor.DarkGray);
            Util.WriteLine("stash:");

            foreach (var item in itemStash)
            {
                item.Info();
            }

            Console.ReadKey();
        }

        public void EquipItem(Type type)
        {
            Item item = null;

            if (type == Helmet.GetType())
            {
                item = Helmet;
            }
            else if (type == Armor.GetType())
            {
                item = Armor;
            }
            else if (type == Legs.GetType())
            {
                item = Legs;
            }
            else if (type == Boots.GetType())
            {
                item = Boots;
            }
            else if (type == Weapon.GetType())
            {
                item = Weapon;
            }
            else
            {
                return;
            }

            int site = 0;
            int maxSite = (ItemStash.Count - 1) / 7;

            while (true)
            {
                List<Item> items = itemStash.Where(x => x.GetType() == type).OrderByDescending(x => x.Level).ToList();

                maxSite = (items.Count - 1) / 7;
                if (site > maxSite)
                {
                    site = maxSite;
                }

                if (items.Count == 0)
                {
                    Util.Write("\nYou don't have any ");
                    Util.Write(type.Name.ToString() + " ", ConsoleColor.DarkGray);
                    Util.Write("in your stash");
                }

                Console.Clear();

                Util.Write("Level", ConsoleColor.DarkRed);
                Util.Write(": ");
                Util.WriteLine(Level + "", ConsoleColor.DarkRed);

                Util.Write(type.Name.ToString() + ": ");
                item.Info(false);

                Util.Write("\n\nItem ", ConsoleColor.DarkGray);
                Util.WriteLine("stash: " + (site + 1) + " / " + (maxSite + 1));

                for (int i = 0; i < 7; ++i)
                {
                    if (items.Count <= i + (site * 7))
                    {
                        break;
                    }

                    Util.Write((i + 1) + " ");
                    items[i + (site * 7)].Info();
                }

                if (site > 0)
                {
                    Util.WriteLine("8. Previous site");
                }

                if (site < maxSite)
                {
                    Util.WriteLine("9. Next site");
                }

                Util.WriteLine("\n0. Exit");

                int decision = Util.NumpadKeyToInt(Console.ReadKey());

                if (decision == 0)
                {
                    break;
                }
                else if (site > 0 && decision == 8)
                {
                    site -= 1;
                }
                else if (site < maxSite && decision == 9)
                {
                    site += 1;
                }
                else if (decision > 0 && decision < 8)
                {
                    if (decision + (site * 7) < items.Count + 1)
                    {
                        if (level >= items[decision - 1 + (site * 7)].Level)
                        {
                            itemStash.Remove(items[decision - 1 + (site * 7)]);
                            if (!item.IsBase)
                            {
                                itemStash.Add(item);
                            }
                            item = items[decision - 1 + (site * 7)];

                            Console.Clear();

                            Util.Write("You've equiped ");
                            Util.WriteLine(item.Name, ConsoleColor.DarkGray);

                            Console.ReadKey();
                        }
                        else
                        {
                            Console.Clear();

                            Util.Write("Your ");
                            Util.Write("level ", ConsoleColor.DarkRed);
                            Util.WriteLine("is too low");

                            Console.ReadKey();
                        }        
                    }
                }

                if (item is Helmet helmet)
                {
                    Helmet = helmet;
                }
                else if (item is Armor armor)
                {
                    Armor = armor;
                }
                else if (item is Legs legs)
                {
                    Legs = legs;
                }
                else if (item is Boots boots)
                {
                    Boots = boots;
                }
                else if (item is Weapon weapon)
                {
                    Weapon = weapon;
                }

                CalculateStats();
            }
        }

        public int ExperienceToLevel(int level)
        {
            int expToAdvance;

            if (level == 0)
            {
                expToAdvance = 10 * ExperienceMultipler;
            }
            else if (level == 1)
            {
                expToAdvance = 20 * ExperienceMultipler;
            }
            else
            {
                expToAdvance = Convert.ToInt32((50 * Math.Pow(level + 1, 3) - 150 * Math.Pow(level + 1, 2) + 400 * (level + 1)) / 
                    (30 / ExperienceMultipler));
            }

            return expToAdvance;
        }

        internal void OrderQuestItemsList()
        {
            QuestItems = QuestItems.OrderBy(x => x.Name).ToList();
        }

        internal void OrderItemStashList()
        {
            ItemStash = ItemStash.OrderBy(x => x.GetType().Name).ThenByDescending(y => y.Level).ToList();
        }

        internal void OrderConsumablesList()
        {
            Consumables = consumables.OrderByDescending(x => x.Level).ThenByDescending(y => y.Health).ToList();
        }

        public int MaxHealth { get => maxHealth; set => maxHealth = value; }
        public int Health { get => health; set => health = value; }
        public Tuple<int, int> Damage { get => damage; set => damage = value; }
        public int Defense { get => defense; set => defense = value; }
        public int Level { get => level; set => level = value; }
        public int Experience { get => experience; set => experience = value; }
        public int Gold { get => gold; set => gold = value; }
        internal List<Item> ItemStash { get => itemStash; set => itemStash = value; }
        internal List<Consumable> Consumables { get => consumables; set => consumables = value; }
        public int Ap { get => ap; set => ap = value; }
        public int ApPerLevel { get => apPerLevel; set => apPerLevel = value; }
        internal List<QuestItem> QuestItems { get => questItems; set => questItems = value; }
        internal List<Quest> QuestsDone { get => questsDone; set => questsDone = value; }
        public int ExperienceMultipler { get => experienceMultipler; set => experienceMultipler = value; }
        internal Helmet Helmet { get => helmet; set => helmet = value; }
        internal Armor Armor { get => armor; set => armor = value; }
        internal Legs Legs { get => legs; set => legs = value; }
        internal Boots Boots { get => boots; set => boots = value; }
        internal Weapon Weapon { get => weapon; set => weapon = value; }

    }
}
