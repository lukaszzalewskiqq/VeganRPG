using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;

namespace VeganRPG
{
    class Game
    {
        string saveName;

        readonly int percentageExperiencePenalty;

        readonly Random randomizer;

        List<Helmet> helmets;
        List<Armor> armors;
        List<Legs> legs;
        List<Boots> boots;
        List<Weapon> weapons;
        List<Consumable> consumables;
        List<Item> items;

        List<Enemy> enemies;

        List<Ability> abilities;

        List<QuestItem> questItems;
        List<Quest> quests;

        List<Area> areas;

        List<NPC> npcs;

        List<City> cities;

        readonly Player player;

        City city;

        List<Quest> activeQuests;
        List<Tuple<Enemy, int>> enemyTracker;

        public Game()
        {
            percentageExperiencePenalty = 3;

            randomizer = new Random();

            GenerateHelmets();
            GenerateArmors();
            GenerateLegs();
            GenerateBoots();
            GenerateWeapons();

            GenerateConsumables();
            GenerateQuestItems();

            GenerateItems();

            GenerateEnemies();

            GenerateAbilities();

            GenerateAreas();
      
            GenerateQuests();

            GenerateNpcs();

            GenerateCities();

            player = new Player(items);
            
            city = cities.Find(x => x.Name == "Farm");

            activeQuests = new List<Quest>();

            enemyTracker = new List<Tuple<Enemy, int>>();
        }

        #region Save / Load
        void Save(string name = null, bool quick = false)
        {
            string save = "";

            SaveGameState(ref save);
            SavePlayerState(ref save);

            if (name != null)
            {
                saveName = name + ".sav";
            }

            File.WriteAllText(saveName, save);

            Console.Clear();

            if (!quick)
            {
                Util.Write("Game ");
                Util.WriteLine("saved", ConsoleColor.DarkRed);
            }

            Console.ReadKey();
        }

        void SaveGameState(ref string save)
        {
            SaveActiveQuests(ref save);
            SaveEnemyTracker(ref save);
            SaveCity(ref save);
        }

        void SaveActiveQuests(ref string save)
        {
            save += "ActiveQuests:";

            foreach (var quest in activeQuests)
            {
                save += quest.Name + ";";
            }

            save += "||\n";
        }

        void SaveEnemyTracker(ref string save)
        {
            save += "EnemyTracker:";

            foreach (var enemy in enemyTracker)
            {
                save += enemy.Item1.Name + "|" + enemy.Item2 + ";";
            }

            save += "||\n";
        }

        void SaveCity(ref string save)
        {
            save += "City:" + city.Name + "||\n";
        }

        void SavePlayerState(ref string save)
        {
            SavePlayerQuestsDone(ref save);

            SavePlayerItems(ref save);
            SavePlayerConsumables(ref save);
            SavePlayerQuestItems(ref save);

            SavePlayerEquipment(ref save);

            SavePlayerStats(ref save);
        }

        void SavePlayerQuestsDone(ref string save)
        {
            save += "QuestsDone:";

            foreach (var quest in player.QuestsDone)
            {
                save += quest.Name + ";";
            }

            save += "||\n";
        }

        void SavePlayerItems(ref string save)
        {
            save += "Items:";

            foreach (var item in player.ItemStash)
            {
                save += item.Name + ";";
            }

            save += "||\n";
        }

        void SavePlayerConsumables(ref string save)
        {
            save += "Consumables:";

            foreach (var item in player.Consumables)
            {
                save += item.Name + "|" + item.Count + ";";
            }

            save += "||\n";
        }

        void SavePlayerQuestItems(ref string save)
        {
            save += "QuestItems:";

            foreach (var item in player.QuestItems)
            {
                save += item.Name + "|" + item.Count + ";";
            }

            save += "||\n";
        }

        void SavePlayerEquipment(ref string save)
        {
            save += "Helmet:" + player.Helmet.Name + "||\n";
            save += "Armor:" + player.Armor.Name + "||\n";
            save += "Legs:" + player.Legs.Name + "||\n";
            save += "Boots:" + player.Boots.Name + "||\n";
            save += "Weapon:" + player.Weapon.Name + "||\n";
        }

        void SavePlayerStats(ref string save)
        {
            save += "Experience:" + player.Experience + "||\n";
            save += "Level:" + player.Level + "||\n";
            save += "Gold:" + player.Gold + "||\n";
            save += "Ap:" + player.Ap + "||\n";
            save += "MaxHealth:" + player.MaxHealth + "||\n";
            save += "Health:" + player.Health + "||\n";
            save += "Damage:" + player.Damage.Item1 + ";" + player.Damage.Item2 + "||\n";
            save += "Defense:" + player.Defense + "||\n";
        }

        bool Load()
        {
            Console.Clear();

            string save = "";

            Util.WriteLine("Adventure name: ");
            string name = Console.ReadLine();

            saveName = name + ".sav";

            if (File.Exists(saveName))
            {
                save = File.ReadAllText(saveName);

                save = save.Replace("\n", "");
                save = save.Trim();
                List<string> saveSplitted = save.Split("||").ToList();
                saveSplitted.RemoveAll(x => x == "");

                LoadGameState(saveSplitted);
                LoadPlayerState(saveSplitted);

                Console.Clear();

                Util.Write("Game ");
                Util.WriteLine("loaded", ConsoleColor.DarkRed);

                Console.ReadKey();

                return true;
            }
            else
            {
                Util.WriteLine("That adventure doesn't exist");

                Console.ReadKey();

                return false;
            }
        }

        static string LoadValue(List<string> saveSplitted, string heading)
        {
            string line = saveSplitted.Find(x => x.StartsWith(heading));

            line = line.Substring(line.IndexOf(":") + 1);

            return line;
        }

        static List<string> LoadValueList(List<string> saveSplitted, string heading)
        {
            List<string> valueList = new List<string>();

            valueList = LoadValue(saveSplitted, heading).Split(";").ToList();
            valueList.RemoveAll(x => x == "");

            return valueList;
        }   

        static List<Tuple<string, int>> LoadValueTupleList(List<string> saveSplitted, string heading)
        {
            List<Tuple<string, int>> valueTupleList = new List<Tuple<string, int>>();
            List<string> valueList = LoadValueList(saveSplitted, heading);

            List<string> tupleString;
            foreach (var value in valueList)
            {
                tupleString = value.Split("|").ToList();
                valueTupleList.Add(new Tuple<string, int>(tupleString[0], Convert.ToInt32(tupleString[1])));
            }

            return valueTupleList;
        }

        void LoadGameState(List<string> saveSplitted)
        {
            LoadActiveQuests(saveSplitted);
            LoadEnemyTracker(saveSplitted);
            LoadCity(saveSplitted);
        }

        void LoadActiveQuests(List<string> saveSplitted)
        {
            List<string> activeQuestsList = LoadValueList(saveSplitted, "ActiveQuests");

            activeQuests = new List<Quest>();
            foreach (var quest in activeQuestsList)
            {
                activeQuests.Add(quests.Find(x => x.Name == quest));
            }
        }
        
        void LoadEnemyTracker(List<string> saveSplitted)
        {
            List<Tuple<string, int>> enemyTrackerList = LoadValueTupleList(saveSplitted, "EnemyTracker");

            enemyTracker = new List<Tuple<Enemy, int>>();
            foreach (var enemy in enemyTrackerList)
            {
                enemyTracker.Add(new Tuple<Enemy, int>(enemies.Find(x => x.Name == enemy.Item1), enemy.Item2));
            }
        }

        void LoadCity(List<string> saveSplitted)
        {
            string cityString = LoadValue(saveSplitted, "City");

            city = cities.Find(x => x.Name == cityString);
        }

        void LoadPlayerState(List<string> saveSplitted)
        {
            LoadPlayerQuestsDone(saveSplitted);

            LoadPlayerItems(saveSplitted);
            LoadPlayerConsumables(saveSplitted);
            LoadPlayerQuestItems(saveSplitted);

            LoadPlayerEquipment(saveSplitted);

            LoadPlayerStats(saveSplitted);
        }

        void LoadPlayerQuestsDone(List<string> saveSplitted)
        {
            List<string> questsDoneList = LoadValueList(saveSplitted, "QuestsDone");

            player.QuestsDone = new List<Quest>();
            foreach (var quest in questsDoneList)
            {
                player.QuestsDone.Add(quests.Find(x => x.Name == quest));
            }
        }

        void LoadPlayerItems(List<string> saveSplitted)
        {
            List<string> itemsList = LoadValueList(saveSplitted, "Items");

            player.ItemStash = new List<Item>();
            foreach (var item in itemsList)
            {
                player.ItemStash.Add(items.Find(x => x.Name == item));
            }
        }

        void LoadPlayerConsumables(List<string> saveSplitted)
        {
            List<Tuple<string, int>> consumablesList = LoadValueTupleList(saveSplitted, "Consumables");

            player.Consumables = new List<Consumable>();
            Consumable consumable;
            foreach (var item in consumablesList)
            {
                consumable = consumables.Find(x => x.Name == item.Item1);
                consumable.Count = item.Item2;
                player.Consumables.Add(consumable);
            }
        }

        void LoadPlayerQuestItems(List<string> saveSplitted)
        {
            List<Tuple<string, int>> questItemsList = LoadValueTupleList(saveSplitted, "QuestItems");

            player.QuestItems = new List<QuestItem>();
            QuestItem questItem;
            foreach (var item in questItemsList)
            {
                questItem = questItems.Find(x => x.Name == item.Item1);
                questItem.Count = item.Item2;
                player.QuestItems.Add(questItem);
            }
        }

        void LoadPlayerEquipment(List<string> saveSplitted)
        {
            string helmetString = LoadValue(saveSplitted, "Helmet");
            string armorString = LoadValue(saveSplitted, "Armor");
            string legsString = LoadValue(saveSplitted, "Legs");
            string bootsString = LoadValue(saveSplitted, "Boots");
            string weaponString = LoadValue(saveSplitted, "Weapon");

            player.Helmet = (Helmet) items.Find(x => x.Name == helmetString);
            player.Armor = (Armor)items.Find(x => x.Name == armorString);
            player.Legs = (Legs)items.Find(x => x.Name == legsString);
            player.Boots = (Boots)items.Find(x => x.Name == bootsString);
            player.Weapon = (Weapon)items.Find(x => x.Name == weaponString);
        }

        void LoadPlayerStats(List<string> saveSplitted)
        {
            player.Experience = Convert.ToInt32(LoadValue(saveSplitted, "Experience"));
            player.Level = Convert.ToInt32(LoadValue(saveSplitted, "Level"));
            player.Gold = Convert.ToInt32(LoadValue(saveSplitted, "Gold"));
            player.Ap = Convert.ToInt32(LoadValue(saveSplitted, "Ap"));
            player.MaxHealth = Convert.ToInt32(LoadValue(saveSplitted, "MaxHealth"));
            player.Health = Convert.ToInt32(LoadValue(saveSplitted, "Health"));
            player.Defense = Convert.ToInt32(LoadValue(saveSplitted, "Defense"));

            List<string> damageList = LoadValueList(saveSplitted, "Damage");
            player.Damage = new Tuple<int, int>(Convert.ToInt32(damageList[0]), Convert.ToInt32(damageList[1]));
        }

        #endregion

        static void ExitGame()
        {
            Console.Clear();

            Util.WriteLine("Exiting the game");

            Console.ReadKey();
        }

        void NewGame()
        {
            saveName = "adventure";

            saveName += randomizer.Next(100000) + ".sav";

            Console.Clear();

            Util.Write("During your trip outside your ");
            Util.Write("homeland", ConsoleColor.Green);
            Util.Write(", you found yourself lost in an unknown ");
            Util.WriteLine("land", ConsoleColor.Green);

            Util.Write("From afar you can notice some buildings, it looks like a ");
            Util.WriteLine("farm", ConsoleColor.Green);

            Util.Write("You chose to go in that direction");

            Console.ReadKey();

            Console.Clear();
        }

        public void StartMenu()
        {
            while (true)
            {
                Console.Clear();

                Util.Write("1. ");
                Util.Write("New ", ConsoleColor.DarkRed);
                Util.WriteLine("game");

                Util.Write("2. ");
                Util.Write("Load ", ConsoleColor.DarkRed);
                Util.WriteLine("game");

                Util.WriteLine("\n0. Exit");

                var decision = Console.ReadKey();

                if (decision.Key == ConsoleKey.NumPad0)
                {
                    ExitGame();

                    return;
                }
                else if (decision.Key == ConsoleKey.NumPad1)
                {
                    NewGame();
                    Menu();
                }
                else if (decision.Key == ConsoleKey.NumPad2)
                {
                    if(Load())
                    {
                        Menu();
                    }
                }             
            }          
        }

        public void Menu()
        {
            while (true)
            {
                if (player.QuestsDone.Find(x => x.Name == "The End") != null)
                {
                    Console.Clear();

                    Util.WriteMulticolor("CONGRATULATIONS, YOU WON THE GAME !!!");
                    Util.Write("\n\n");

                    Util.Write("Experience", ConsoleColor.DarkRed);
                    Util.Write(": ");
                    Util.Write(player.Experience + " ", ConsoleColor.DarkRed);
                    Util.Write("/ ");
                    Util.WriteLine(player.ExperienceToLevel(player.Level) + "", ConsoleColor.DarkRed);

                    Util.Write("Level", ConsoleColor.DarkRed);
                    Util.Write(": ");
                    Util.WriteLine(player.Level + "\n", ConsoleColor.DarkRed);

                    Util.Write("Gold", ConsoleColor.DarkYellow);
                    Util.Write(": ");
                    Util.WriteLine(player.Gold + "\n", ConsoleColor.DarkYellow);

                    Util.Write("Health", ConsoleColor.Red);
                    Util.Write(": ");
                    Util.Write(player.Health + "", ConsoleColor.Red);
                    Util.Write(" / ");
                    Util.WriteLine(player.MaxHealth + "", ConsoleColor.Red);

                    Util.Write("Damage", ConsoleColor.Magenta);
                    Util.Write(": ");
                    Util.Write(player.Damage.Item1 + "", ConsoleColor.Magenta);
                    Util.Write(" - ");
                    Util.WriteLine(player.Damage.Item2 + "", ConsoleColor.Magenta);

                    Util.Write("Defense", ConsoleColor.DarkMagenta);
                    Util.Write(": ");
                    Util.WriteLine(player.Defense + "\n", ConsoleColor.DarkMagenta);

                    Save(saveName, true);

                    Console.ReadKey();                  

                    break;
                }

                Console.Clear();

                Util.WriteLine(city.Name, ConsoleColor.Green);

                Util.Write("1. ");
                Util.WriteLine("People", ConsoleColor.Blue);

                Util.WriteLine("2. Go outside ");

                Util.Write("\n3. ");
                Util.Write("Statistics ", ConsoleColor.DarkGreen);
                Util.Write("and ");
                Util.WriteLine("Equipment", ConsoleColor.DarkGray);

                Util.Write("4. ");
                Util.Write("Quest", ConsoleColor.Red);
                Util.WriteLine(" tracker");

                if (city.EndQuest != null && player.QuestsDone.Find(x => x == city.EndQuest) != null)
                {
                    Util.Write("5. Travel to the next ");
                    Util.WriteLine("place", ConsoleColor.Green);
                }

                Util.Write("\n6. ");
                Util.WriteLine("Save", ConsoleColor.DarkRed);

                Util.Write("7. ");
                Util.Write("Save ", ConsoleColor.DarkRed);
                Util.WriteLine("as");

                Util.WriteLine("\nE. Exit");

                var decision = Console.ReadKey();

                if (decision.Key == ConsoleKey.E)
                {
                    while (true)
                    {
                        Console.Clear();

                        Util.Write("Should the game be ");
                        Util.Write("saved ", ConsoleColor.DarkRed);
                        Util.WriteLine("before you close the current game?");

                        Util.Write("1. ");
                        Util.WriteLine("Save", ConsoleColor.DarkRed);

                        Util.Write("\n0. Exit");

                        decision = Console.ReadKey();

                        if (decision.Key == ConsoleKey.NumPad0)
                        {
                            return;
                        }
                        else if (decision.Key == ConsoleKey.NumPad1)
                        {
                            Save();
                        }
                    }
                }
                else if (decision.Key == ConsoleKey.NumPad1)
                {
                    city.People(player, activeQuests, enemyTracker);
                }
                else if (decision.Key == ConsoleKey.NumPad2)
                {
                    while (true)
                    {
                        if (player.QuestsDone.Find(x => x.Name == "The End") != null)
                        {
                            break;
                        }

                        Enemy enemy = city.Outside(player, activeQuests);

                        if (enemy != null)
                        {
                            if (Fight(enemy))
                            {
                                if (enemy.Boss)
                                {
                                    Quest bossQuest = null;

                                    foreach (var quest in activeQuests)
                                    {
                                        if (quest.QuestEnemies.Find(x => x.Item1 == enemy) != null)
                                        {
                                            bossQuest = quest;
                                        }
                                    }
                                    if (bossQuest != null)
                                    {
                                        Console.Clear();

                                        bossQuest.Finish(player, enemyTracker);
                                        activeQuests.Remove(bossQuest);
                                    }
                                }

                                continue;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else if (decision.Key == ConsoleKey.NumPad3)
                {
                    PlayerMenu();
                }
                else if (decision.Key == ConsoleKey.NumPad4)
                {
                    QuestTracker();
                }
                else if (decision.Key == ConsoleKey.NumPad5)
                {
                    if (city.EndQuest != null && player.QuestsDone.Find(x => x == city.EndQuest) != null)
                    {
                        Travel();
                    }
                }
                else if (decision.Key == ConsoleKey.NumPad6)
                {
                    Save();
                }
                else if (decision.Key == ConsoleKey.NumPad7)
                {
                    Console.Clear();

                    Util.WriteLine("Adventure name: ");

                    string name = Console.ReadLine();

                    Save(name);
                }
            }
        }

        void PlayerMenu()
        {
            while (true)
            {
                Console.Clear();
                Util.Write("1. ");
                Util.WriteLine("Statistics", ConsoleColor.DarkGreen);

                Util.Write("\n2. ");
                Util.Write("Item ", ConsoleColor.DarkGray);
                Util.WriteLine("stash");

                Util.Write("3. Equip ");
                Util.WriteLine("Item", ConsoleColor.DarkGray);

                Util.Write("\n4. ");
                Util.WriteLine("Consume", ConsoleColor.Yellow);

                Util.WriteLine("\n0. Exit");

                ConsoleKeyInfo decision = Console.ReadKey();

                if (decision.Key == ConsoleKey.NumPad0)
                {
                    break;
                }
                else if (decision.Key == ConsoleKey.NumPad1)
                {
                    player.ShowStatistics();
                }
                else if (decision.Key == ConsoleKey.NumPad2)
                {
                    player.ShowItemStash();
                }
                else if (decision.Key == ConsoleKey.NumPad3)
                {
                    while (true)
                    {
                        Console.Clear();

                        Util.Write("1. Equip ");
                        Util.WriteLine("Helmet", ConsoleColor.DarkGray);

                        Util.Write("2. Equip ");
                        Util.WriteLine("Armor", ConsoleColor.DarkGray);

                        Util.Write("3. Equip ");
                        Util.WriteLine("Legs", ConsoleColor.DarkGray);

                        Util.Write("4. Equip ");
                        Util.WriteLine("Boots", ConsoleColor.DarkGray);

                        Util.Write("5. Equip ");
                        Util.WriteLine("Weapon", ConsoleColor.DarkGray);

                        Util.WriteLine("\n0. Exit");

                        decision = Console.ReadKey();

                        if (decision.Key == ConsoleKey.NumPad0)
                        {
                            break;
                        }
                        else if (decision.Key == ConsoleKey.NumPad1)
                        {
                            player.EquipItem(new Helmet().GetType());
                        }
                        else if (decision.Key == ConsoleKey.NumPad2)
                        {
                            player.EquipItem(new Armor().GetType());
                        }
                        else if (decision.Key == ConsoleKey.NumPad3)
                        {
                            player.EquipItem(new Legs().GetType());
                        }
                        else if (decision.Key == ConsoleKey.NumPad4)
                        {
                            player.EquipItem(new Boots().GetType());
                        }
                        else if (decision.Key == ConsoleKey.NumPad5)
                        {
                            player.EquipItem(new Weapon().GetType());
                        }
                    }
                }
                else if (decision.Key == ConsoleKey.NumPad4)
                {
                    int site = 0;
                    while (true)
                    {
                        site = Consume(site);
                        
                        if (site < 0)
                        {
                            break;
                        }
                    }
                }
            }
        }

        void Travel()
        {
            city = city.NextCity;

            activeQuests = new List<Quest>();
            enemyTracker = new List<Tuple<Enemy, int>>();
        }

        void QuestTracker()
        {
            Console.Clear();

            if (activeQuests.Count > 0)
            {
                foreach (var quest in activeQuests)
                {
                    quest.Info(player, enemyTracker);
                    Util.Write("\n\n");
                }
            }
            else
            {
                Util.Write("You don't have any active ");
                Util.WriteLine("quest", ConsoleColor.Red);
            }

            Console.ReadKey();
        }    

        int Consume(int lastSite = 0)
        {
            player.OrderConsumablesList();

            if (player.Consumables.Count == 0)
            {
                Console.Clear();

                Util.Write("You don't have anything to ");
                Util.WriteLine("consume", ConsoleColor.Yellow);

                Console.ReadKey();

                return -1;
            }

            int site = lastSite;
            int maxSite;

            while (true)
            {
                maxSite = (player.Consumables.Count - 1) / 7;
                if (site > maxSite)
                {
                    site = maxSite;
                }

                Console.Clear();

                Util.Write("Level", ConsoleColor.DarkRed);
                Util.Write(": ");
                Util.WriteLine(player.Level + "", ConsoleColor.DarkRed);

                Util.Write("Health", ConsoleColor.Red);
                Util.Write(": ");
                Util.Write(player.Health + " ", ConsoleColor.Red);
                Util.Write("/ ");
                Util.WriteLine(player.MaxHealth + "", ConsoleColor.Red);

                Util.Write("Ability points", ConsoleColor.Cyan);
                Util.Write(": ");
                Util.Write(player.Ap + " ", ConsoleColor.Cyan);
                Util.Write("/ ");
                Util.WriteLine((player.Level * player.ApPerLevel) + "\n", ConsoleColor.Cyan);

                Util.Write("Consumables", ConsoleColor.Yellow);
                Util.WriteLine(": " + (site + 1) + " / " + (maxSite + 1) + "");
                for (int i = 0; i < 7; ++i)
                {
                    if (player.Consumables.Count <= i + (site * 7))
                    {
                        break;
                    }

                    Util.Write(i + 1 + " ");
                    player.Consumables[i + (site * 7)].Info(false);
                    Util.Write(" - " + player.Consumables[i + (site * 7)].Count + "\n");
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

                Console.Clear();

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
                    if (decision + (site * 7) < player.Consumables.Count + 1)
                    {
                        if (player.Level < player.Consumables[decision - 1 + (site * 7)].Level)
                        {
                            Util.Write("Your ");
                            Util.Write("level ", ConsoleColor.DarkRed);
                            Util.Write("is too low to ");
                            Util.Write("consume ", ConsoleColor.Yellow);
                            Util.WriteLine("this");

                            Console.ReadKey();
                        }
                        else
                        {
                            int addedHealth;
                            int addedAp;

                            if (player.Health + player.Consumables[decision - 1 + (site * 7)].Health > player.MaxHealth)
                            {
                                addedHealth = player.MaxHealth - player.Health;
                            }
                            else
                            {
                                addedHealth = player.Consumables[decision - 1 + (site * 7)].Health;
                            }

                            if (player.Ap + player.Consumables[decision - 1 + (site * 7)].Ap > player.Level * player.ApPerLevel)
                            {
                                addedAp = (player.Level * player.ApPerLevel) - player.Ap;

                            }
                            else
                            {
                                addedAp = player.Consumables[decision - 1 + (site * 7)].Ap;
                            }

                            if (addedHealth == 0 && addedAp == 0)
                            {
                                Util.Write("You don't need to ");
                                Util.WriteLine("consume", ConsoleColor.Yellow);

                                Console.ReadKey();

                                return -1;
                            }
                            else
                            {
                                player.Health += addedHealth;
                                player.Ap += addedAp;
                            }

                            Util.Write("You consumed ");
                            Util.WriteLine(player.Consumables[decision - 1 + (site * 7)].Name, ConsoleColor.Yellow);
                            if (addedHealth > 0)
                            {
                                Util.Write("You got ");
                                Util.WriteLine(addedHealth + " Health", ConsoleColor.Red);
                            }
                            if (addedAp > 0)
                            {
                                Util.Write("You got ");
                                Util.WriteLine(addedAp + " AP", ConsoleColor.Cyan);
                            }
                            Console.ReadKey();

                            if (player.Consumables[decision - 1 + (site * 7)].Count == 1)
                            {
                                player.Consumables.Remove(player.Consumables[decision - 1 + (site * 7)]);
                            }
                            else
                            {
                                player.Consumables[decision - 1 + (site * 7)].Count -= 1;
                            }

                            return site;
                        }
                    }
                }
            }

            return -1;
        }

        bool UseAbility(List<Ability> possibleAbilities, ref List<Ability> abilitiesInUse, ref AbilityEffects abilityEffectsCombined)
        {
            if (possibleAbilities.Count == 0)
            {
                Util.Write("You can't use any ");
                Util.WriteLine("ability", ConsoleColor.Cyan);

                Console.ReadKey();

                return false;
            }
            int site = 0;
            int maxSite;

            while (true)
            {
                maxSite = (possibleAbilities.Count - 1) / 7;
                if (site > maxSite)
                {
                    site = maxSite;
                }

                Console.Clear();

                Util.Write("Health", ConsoleColor.Red);
                Util.Write(": ");
                Util.Write(player.Health + " ", ConsoleColor.Red);
                Util.Write("/ ");
                Util.WriteLine(player.MaxHealth + "", ConsoleColor.Red);

                Util.Write("Damage", ConsoleColor.Magenta);
                Util.Write(": ");
                Util.Write(player.Damage.Item1 + " ", ConsoleColor.Magenta);
                Util.Write("- ");
                Util.WriteLine(player.Damage.Item2 + "", ConsoleColor.Magenta);

                Util.Write("Defense", ConsoleColor.DarkMagenta);
                Util.Write(": ");
                Util.WriteLine(player.Defense + "", ConsoleColor.DarkMagenta);

                Util.Write("Ability points", ConsoleColor.Cyan);
                Util.Write(": ");
                Util.Write(player.Ap + " ", ConsoleColor.Cyan);
                Util.Write("/ ");
                Util.WriteLine((player.Level * player.ApPerLevel) + "\n", ConsoleColor.Cyan);

                Util.Write("Abilities", ConsoleColor.Cyan);
                Util.WriteLine(":\n");
                for (int i = 0; i < 7; ++i)
                {
                    if (possibleAbilities.Count <= i + (site * 7))
                    {
                        break;
                    }

                    Util.Write(i + 1 + " ");
                    possibleAbilities[i + (site * 7)].Info();
                    Util.Write("\n");
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

                Console.Clear();

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
                    if (decision + (site * 7) < possibleAbilities.Count + 1)
                    {
                        if (player.Ap < possibleAbilities[decision - 1 + (site * 7)].Cost)
                        {
                            Util.Write("Your don't have enough ");
                            Util.WriteLine("ability points", ConsoleColor.Cyan);

                            Console.ReadKey();
                        }
                        else
                        {
                            Util.Write("You used ");
                            Util.Write(possibleAbilities[decision - 1 + (site * 7)].Name + " ", ConsoleColor.Cyan);
                            Util.Write("for ");
                            Util.WriteLine(possibleAbilities[decision - 1 + (site * 7)].Cost + " ability points", ConsoleColor.Cyan);

                            Console.ReadKey();

                            player.Ap -= possibleAbilities[decision - 1 + (site * 7)].Cost;

                            abilitiesInUse.Add(possibleAbilities[decision - 1 + (site * 7)]);

                            possibleAbilities[decision - 1 + (site * 7)].Use(ref abilityEffectsCombined);
                            possibleAbilities.Remove(possibleAbilities[decision - 1 + (site * 7)]);

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        bool Fight(Enemy enemy)
        {
            bool won = false;

            Tuple<int, int> playerDamage = player.Damage;
            int playerDefense = player.Defense;

            Tuple<int, int> enemyDamage = enemy.Damage;
            int enemyDefense = enemy.Defense;
            Tuple<int, int> enemyGold = enemy.Gold;
            int enemyExperience = enemy.Experience;

            int maxTimesConsuming = (player.Level / 4) + 1;
            int maxTimesUsingAbility = (player.Level / 4) + 1;

            int timesConsumed = 0;
            int abilitiesUsed = 0;

            List<Ability> learntAbilities = abilities.Where(x => player.Level >= x.Level).
                Where(y => player.Level <= y.MaxLevel).ToList();
            List<Ability> abilitiesInUse = new List<Ability>();
            AbilityEffects abilityEffectsCombined = new AbilityEffects();

            while (true)
            {
                Console.Clear();

                Util.WriteLine("You");

                Util.Write("Health", ConsoleColor.Red);
                Util.Write(": ");
                Util.Write(player.Health + " ", ConsoleColor.Red);
                Util.Write("/ ");
                Util.WriteLine(player.MaxHealth + "", ConsoleColor.Red);

                Util.Write("Damage", ConsoleColor.Magenta);
                Util.Write(": ");
                Util.Write(player.Damage.Item1 + " ", ConsoleColor.Magenta);
                Util.Write("- ");
                Util.WriteLine(player.Damage.Item2 + "", ConsoleColor.Magenta);

                Util.Write("Defense", ConsoleColor.DarkMagenta);
                Util.Write(": ");
                Util.WriteLine(player.Defense + "", ConsoleColor.DarkMagenta);

                Util.Write("Ability points", ConsoleColor.Cyan);
                Util.Write(": ");
                Util.Write(player.Ap + " ", ConsoleColor.Cyan);
                Util.Write("/ ");
                Util.WriteLine((player.Level * player.ApPerLevel) + "\n", ConsoleColor.Cyan);

                Util.WriteLine(enemy.Name + "", ConsoleColor.Blue);

                Util.Write("Health", ConsoleColor.Red);
                Util.Write(": ");
                Util.Write(enemy.Health + " ", ConsoleColor.Red);
                Util.Write("/ ");
                Util.WriteLine(enemy.MaxHealth + "", ConsoleColor.Red);

                Util.Write("Damage", ConsoleColor.Magenta);
                Util.Write(": ");
                Util.Write(enemy.Damage.Item1 + " ", ConsoleColor.Magenta);
                Util.Write("- ");
                Util.WriteLine(enemy.Damage.Item2 + "", ConsoleColor.Magenta);

                Util.Write("Defense", ConsoleColor.DarkMagenta);
                Util.Write(": ");
                Util.WriteLine(enemy.Defense + " ", ConsoleColor.DarkMagenta);

                Util.WriteLine("\n\n1. Hit");
                if (timesConsumed < maxTimesConsuming && player.Consumables.Count > 0)
                {
                    Util.Write("2. ");
                    Util.WriteLine("Consume", ConsoleColor.Yellow);
                }
                if (abilitiesUsed < maxTimesUsingAbility && learntAbilities.Count > 0)
                {
                    Util.Write("3. Use ");
                    Util.WriteLine("ability", ConsoleColor.Cyan);
                }
                var decision = Console.ReadKey();

                Util.Write("\n\n");

                if (decision.Key == ConsoleKey.NumPad1)
                {
                    PlayerHitEnemy(enemy);
                }
                else if (decision.Key == ConsoleKey.NumPad2 && timesConsumed < maxTimesConsuming && player.Consumables.Count > 0)
                {
                    if (Consume() >= 0)
                    {
                        timesConsumed += 1;
                    }

                    continue;
                }
                else if (decision.Key == ConsoleKey.NumPad3 && abilitiesUsed < maxTimesUsingAbility && learntAbilities.Count > 0)
                {
                    if (UseAbility(learntAbilities, ref abilitiesInUse, ref abilityEffectsCombined))
                    {
                        abilitiesUsed += 1;

                        abilityEffectsCombined.ApplyEffects(player, playerDefense, playerDamage,
                                enemy, enemyDefense, enemyDamage, enemyGold, enemyExperience);
                    }

                    continue;
                }
                else
                {
                    continue;
                }

                if (enemy.Health <= 0)
                {
                    PlayerWinningAward(enemy);
                    won = true;

                    break;
                }

                EnemyHitPlayer(enemy);

                if (player.Health <= 0)
                {
                    PlayerLosingPenalty(enemy);

                    break;
                }

                Console.ReadKey();

                foreach (var ability in abilitiesInUse)
                {
                    if (ability.Turns > 0)
                    {
                        ability.Turns -= 1;

                        if (ability.Turns == 0)
                        {
                            ability.Stop(ref abilityEffectsCombined);

                            abilityEffectsCombined.ApplyEffects(player, playerDefense, playerDamage,
                                enemy, enemyDefense, enemyDamage, enemyGold, enemyExperience);
                        }
                    }
                }      
            }

            player.Damage = playerDamage;
            player.Defense = playerDefense;

            enemy.Health = enemy.MaxHealth;

            enemy.Damage = enemyDamage;
            enemy.Defense = enemyDefense;
            enemy.Gold = enemyGold;
            enemy.Experience = enemyExperience;

            foreach (var ability in abilities)
            {
                ability.Turns = ability.TurnsConst;
            }

            return won;
        }

        void PlayerHitEnemy(Enemy enemy)
        {
            int enemyDefense = enemy.Defense;

            if (enemyDefense < -90)
            {
                enemyDefense = -90;
            }

            double defense = 100.0 / (100.0 + enemyDefense);
            int damage;

            if (player.Damage.Item1 < player.Damage.Item2)
            {
                damage = Convert.ToInt32(randomizer.Next(player.Damage.Item1, player.Damage.Item2 + 1) * defense);
            }
            else
            {
                damage = Convert.ToInt32(randomizer.Next(player.Damage.Item2, player.Damage.Item1 + 1) * defense);
            }

            if (enemy.Health - damage > enemy.MaxHealth)
            {
                enemy.Health = enemy.MaxHealth;
            }
            else
            {
                enemy.Health -= damage;
            }

            Util.Write("You did ");
            Util.Write(damage + " damage ", ConsoleColor.Magenta);
            Util.Write("to ");
            Util.WriteLine(enemy.Name, ConsoleColor.Blue);
        }

        void EnemyHitPlayer(Enemy enemy)
        {
            int playerDefense = player.Defense;

            if (playerDefense < -90)
            {
                playerDefense = -90;
            }

            double defense = 100.0 / (100.0 + playerDefense);
            int damage;

            if (enemy.Damage.Item1 < enemy.Damage.Item2)
            {
                damage = Convert.ToInt32(randomizer.Next(enemy.Damage.Item1, enemy.Damage.Item2 + 1) * defense);
            }
            else
            {
                damage = Convert.ToInt32(randomizer.Next(enemy.Damage.Item2, enemy.Damage.Item1 + 1) * defense);
            }

            if (player.Health - damage > player.MaxHealth)
            {
                player.Health = player.MaxHealth;
            }
            else
            {
                player.Health -= damage;
            }


            Util.Write(enemy.Name, ConsoleColor.Blue);
            Util.Write(" did ");
            Util.Write(damage + " damage ", ConsoleColor.Magenta);
            Console.WriteLine("to you");
        }

        void PlayerWinningAward(Enemy enemy)
        {
            player.Experience += enemy.Experience;

            int gold = randomizer.Next(enemy.Gold.Item1, enemy.Gold.Item2 + 1);
            player.Gold += gold;

            int number;
            List<Item> loot = new List<Item>();
            foreach (var item in enemy.Loot)
            {
                number = randomizer.Next(0, item.Item2);

                if (number == 0)
                {
                    loot.Add(item.Item1);
                }
            }

            player.ItemStash.AddRange(loot.Where(x => x is not Consumable && x is not QuestItem));

            List<Consumable> consumablesLooted = loot.Where(x => x is Consumable).Cast<Consumable>().ToList();
            foreach (var item in consumablesLooted)
            {
                if (player.Consumables.Contains(item))
                {
                    player.Consumables.Find(x => x == item).Count += 1;
                }
                else
                {
                    item.Count = 1;
                    player.Consumables.Add(item);
                }
            }

            List<QuestItem> questItemsLooted = loot.Where(x => x is QuestItem).Cast<QuestItem>().ToList();
            QuestItem questItem;
            foreach (var item in questItemsLooted)
            {
                loot.Remove(item);

                foreach (var quest in activeQuests)
                {
                    if (quest.QuestItems.Count > 0)
                    {
                        if (quest.QuestItems.Exists(x => x.Item1 == item))
                        {
                            if (player.QuestItems.Contains(item))
                            {
                                questItem = player.QuestItems.Find(x => x == item);

                                if (questItem.Count < quest.QuestItems.Find(x => x.Item1 == questItem).Item2)
                                {
                                    loot.Add(item);
                                    questItem.Count += 1;
                                }
                            }
                            else
                            {
                                loot.Add(item);
                                item.Count = 1;
                                player.QuestItems.Add(item);
                            }
                        }
                    }
                }
            }

            if (activeQuests.Count > 0)
            {
                Tuple<Enemy, int> trackedEnemy;

                trackedEnemy = enemyTracker.Find(x => x.Item1 == enemy);
                
                if (trackedEnemy != null)
                {
                    enemyTracker[enemyTracker.FindIndex(x => x.Item1 == enemy)] = new Tuple<Enemy, int>(enemy, trackedEnemy.Item2 + 1);
                }
            }

            Util.Write("\nYou won figth against ");
            Util.WriteLine(enemy.Name + "", ConsoleColor.Blue);

            Util.Write("You earned ");
            Util.WriteLine(enemy.Experience + " experience", ConsoleColor.DarkRed);

            Util.Write("You gained ");
            Util.WriteLine(gold + " gold", ConsoleColor.DarkYellow);

            if (loot.Count > 0)
            {
                Util.Write("You ");
                Util.Write("looted", ConsoleColor.DarkGray);
                Util.WriteLine(":");
            }
            foreach (var item in loot)
            {
                item.Info();
            }

            if (player.CalculateStats())
            {
                Util.Write("\nYou advanced to ");
                Util.WriteLine("level " + player.Level, ConsoleColor.DarkRed);

                Util.Write("You have regained your ");
                Util.WriteLine("health", ConsoleColor.Red);

                player.Health = player.MaxHealth;
            }

            Util.WriteLine("\n0. Exit");

            while (true)
            {
                int decision = Util.NumpadKeyToInt(Console.ReadKey());

                if (decision == 0)
                {
                    break;
                }
            }
        }

        void PlayerLosingPenalty(Enemy enemy)
        {
            double runawayHealth = 0.25;
            int lostExperience = Convert.ToInt32((player.Experience / 100.0) * percentageExperiencePenalty);

            Util.Write("\n" + enemy.Name + " ", ConsoleColor.Blue);
            Util.WriteLine("defeated you");

            Util.Write("You ran away with little to no ");
            Util.WriteLine("health", ConsoleColor.Red);

            Util.Write("\nYou lost ");
            Util.WriteLine(lostExperience + " experience", ConsoleColor.DarkRed);

            player.Experience -= lostExperience;
            player.Health = Convert.ToInt32(player.MaxHealth * runawayHealth);

            if (player.CalculateStats())
            {
                Util.Write("\nYou dropped to ");
                Util.WriteLine("level " + player.Level, ConsoleColor.DarkRed);
            }

            Util.WriteLine("0. Exit");

            while (true)
            {
                int decision = Util.NumpadKeyToInt(Console.ReadKey());

                if (decision == 0)
                {
                    break;
                }
            }
        }

        void GenerateHelmets()
        {
            helmets = new List<Helmet>()
            {
                #region Farm
                new Helmet(0, "Head", 0, 0, true),
                new Helmet(0, "Worn Leather Helmet", 5, 0),
                new Helmet(1, "Leather Helmet", 8, 0),
                new Helmet(5, "Quilted Helmet", 15, 3),
                new Helmet(8, "Worn Bronze Helmet", 35, 6),
                new Helmet(10, "Bronze Helmet", 50, 9),
                #endregion

                #region Amfurce
                new Helmet(14, "Artificial Leather Helmet", 110, 20),
                new Helmet(19, "Cruelty-Free Leather Helmet", 200, 25),
                new Helmet(22, "BIO Leather Helmet", 230, 38),
                new Helmet(26, "Empathy Helmet", 350, 75),
                new Helmet(30, "BIO Helmet of Flowing Water", 700, 150),
                #endregion
            };
        }

        void GenerateArmors()
        {
            armors = new List<Armor>
            {
                #region Farm
                new Armor(0, "Torso", 0, 0, true),
                new Armor(1, "Worn Leather Armor", 10, 0),
                new Armor(2, "Leather Armor", 22, 0),
                new Armor(6, "Quilted Armor", 40, 5),
                new Armor(10, "Worn Bronze Armor", 80, 12),
                new Armor(12, "Bronze Armor", 140, 20),
                #endregion

                #region Amfurce
                new Armor(16, "Artificial Leather Armor", 230, 40),
                new Armor(21, "Cruelty-Free Leather Armor", 350, 50),
                new Armor(24, "BIO Leather Armor", 400, 65),
                new Armor(28, "Empathy Armor", 600, 130),
                new Armor(30, "BIO Armor of Flowing Water", 1000, 200)
                #endregion
            };
        }

        void GenerateLegs()
        {
            legs = new List<Legs>()
            {
                #region Farm
                new Legs(0, "Legs", 0, 0, true),
                new Legs(1, "Worn Leather Legs", 8, 0),
                new Legs(2, "Leather Legs", 15, 0),
                new Legs(5, "Quilted Legs", 25, 4),
                new Legs(9, "Worn Bronze Legs", 55, 9),
                new Legs(11, "Bronze Legs", 75, 13),
                #endregion

                #region Amfurce
                new Legs(15, "Artificial Leather Legs", 120, 25),
                new Legs(20, "Cruelty-Free Leather Legs", 200, 30),
                new Legs(23, "BIO Leather Legs", 240, 42),
                new Legs(27, "Empathy Legs", 350, 80),
                new Legs(30, "BIO Legs of Flowing Water", 800, 150)
                #endregion
            };
        }

        void GenerateBoots()
        {
            boots = new List<Boots>
            {
                #region Farm
                new Boots(0, "Feet", 0, 0, true),
                new Boots(0, "Worn Leather Boots", 3, 0),
                new Boots(1, "Leather Boots", 6, 0),
                new Boots(4, "Quilted Boots", 12, 3),
                new Boots(7, "Worn Bronze Boots", 25, 7),
                new Boots(9, "Bronze Boots", 40, 8),
                #endregion

                #region Amfurce
                new Boots(13, "Artificial Leather Boots", 70, 15),
                new Boots(18, "Cruelty-Free Leather Boots", 130, 20),
                new Boots(21, "BIO Leather Boots", 150, 30),
                new Boots(25, "Empathy Boots", 230, 60),
                new Boots(30, "BIO Boots of Flowing Water", 500, 100)
                #endregion
            };
        }

        void GenerateWeapons()
        {
            weapons = new List<Weapon>
            {
                #region Farm
                new Weapon(0, "Hands", new Tuple<int, int>(1, 1), true),
                new Weapon(1, "Wooden Stick", new Tuple<int, int>(1, 2)),
                new Weapon(2, "Rock", new Tuple<int, int>(1, 3)),
                new Weapon(3, "Branch", new Tuple<int, int>(3, 4)),
                new Weapon(5, "Wooden Sword", new Tuple<int, int>(3, 9)),
                new Weapon(7, "Wooden Axe", new Tuple<int, int>(2, 13)),
                new Weapon(8, "Wooden Well Carved Sword", new Tuple<int, int>(6, 14)),
                new Weapon(10, "Used Bronze Sword", new Tuple<int, int>(15, 20)),
                new Weapon(11, "Bronze Sword", new Tuple<int, int>(15, 25)),
                new Weapon(13, "Bronze Axe", new Tuple<int, int>(8, 48)),
                #endregion

                #region Amfurce
                new Weapon(16, "BIO Wooden Stick", new Tuple<int, int>(30, 40)),
                new Weapon(19, "BIO Wooden Sword", new Tuple<int, int>(31, 51)),
                new Weapon(22, "BIO Wooden Axe", new Tuple<int, int>(25, 75)),
                new Weapon(25, "Empathy Sword", new Tuple<int, int>(50, 70)),
                new Weapon(30, "BIO Staff of Flowing Water", new Tuple<int, int>(100, 150)),
                new Weapon(30, "BIO Leather Ball", new Tuple<int, int>(500, 1000))
                #endregion
            };
        }

        void GenerateConsumables()
        {
            #region Farm
            consumables = new List<Consumable>();
            consumables.Add(new Consumable(0, "Grain of Rice", 1, 0));
            consumables.Add(new Consumable(0, "Tomato Seed", 1, 0));
            consumables.Add(new Consumable(0, "Pumpkin Seed", 2, 0));
            consumables.Add(new Consumable(0, "Chia Seed", 3, 0));
            consumables.Add(new Consumable(1, "Peanut", 6, 0));
            consumables.Add(new Consumable(1, "Walnut", 9, 0));
            consumables.Add(new Consumable(2, "Wheat", 17, 0));
            consumables.Add(new Consumable(2, "Millet", 22, 0));
            consumables.Add(new Consumable(3, "Rice", 30, 0));

            consumables.Add(new Consumable(2, "Tobacco Leaf", 0, 2));
            consumables.Add(new Consumable(4, "Apple Juice", 15, 3));

            consumables.Add(new Consumable(4, "Black Widow Venom", 0, 20));
            consumables.Add(new Consumable(4, "Black Widow Leg", 150, 0));

            consumables.Add(new Consumable(4, "White Beans", 35, 0));
            consumables.Add(new Consumable(5, "Kidney Beans", 50, 0));
            consumables.Add(new Consumable(6, "Black Beans", 80, 0));
            consumables.Add(new Consumable(6, "Banana", 40, 5));
            consumables.Add(new Consumable(6, "Crab Spider Leg", 0, 9));
            consumables.Add(new Consumable(6, "Orange Spider Venom", 75, 4));
            consumables.Add(new Consumable(6, "Bowl of Spider Soup", 250, 25));

            consumables.Add(new Consumable(7, "Tomato", 60, 5));
            consumables.Add(new Consumable(7, "Cucumber", 50, 7));
            consumables.Add(new Consumable(7, "Aubergine", 0, 13));
            consumables.Add(new Consumable(8, "Courgette", 160, 0));
            consumables.Add(new Consumable(9, "Apple", 175, 5));
            consumables.Add(new Consumable(10, "Pear", 280, 0));
            consumables.Add(new Consumable(10, "Truffles", 0, 28));

            consumables.Add(new Consumable(12, "Farmenstein", 400, 25));
            #endregion

            #region Amfurce
            consumables.Add(new Consumable(11, "Vegan Cheese Slice", 300, 0));
            consumables.Add(new Consumable(12, "Vegan Slice of Ham", 250, 10));
            consumables.Add(new Consumable(12, "Bread", 50, 35));
            consumables.Add(new Consumable(12, "Roll", 250, 15));
            consumables.Add(new Consumable(13, "Vegan Butter", 300, 15));
            consumables.Add(new Consumable(14, "Salt", 0, 45));
            consumables.Add(new Consumable(15, "Pepper", 0, 50));

            consumables.Add(new Consumable(16, "Vegan Sandwich", 750, 75));

            consumables.Add(new Consumable(17, "Orange", 600, 0));
            consumables.Add(new Consumable(18, "Grape", 500, 15));
            consumables.Add(new Consumable(18, "Tofu", 400, 30));
            consumables.Add(new Consumable(19, "Strawberry", 750, 0));
            consumables.Add(new Consumable(20, "Raspberry", 650, 15));
            consumables.Add(new Consumable(21, "Blueberry", 200, 65));
            consumables.Add(new Consumable(22, "Mushrooms", 800, 10));
            consumables.Add(new Consumable(22, "Smoked Paprika", 0, 95));
            consumables.Add(new Consumable(23, "Soy Milk", 1000, 0));
            consumables.Add(new Consumable(23, "Rice Milk", 500, 50));
            consumables.Add(new Consumable(24, "Oat Milk", 250, 85));
            consumables.Add(new Consumable(25, "Cashew Milk", 1200, 0));
            consumables.Add(new Consumable(25, "Almond Milk", 600, 60));
            consumables.Add(new Consumable(26, "Seitan", 900, 40));
            consumables.Add(new Consumable(27, "Soy Protein", 400, 100));

            consumables.Add(new Consumable(25, "Tempeh Stir Fry", 2000, 150));
            consumables.Add(new Consumable(30, "BIO Tofu", 3000, 200));
            #endregion
        }

        void GenerateItems()
        {
            items = new List<Item>();

            items.AddRange(helmets);
            items.AddRange(armors);
            items.AddRange(legs);
            items.AddRange(boots);
            items.AddRange(weapons);

            items.AddRange(consumables);
            items.AddRange(questItems);
        }

        void GenerateEnemies()
        {
            #region Farm
            List<Tuple<Item, int>> antLoot = new List<Tuple<Item, int>> 
            {
                new Tuple<Item, int>(items.Find(x => x.Name == "Grain of Rice"), 2),
                new Tuple<Item, int>(items.Find(x => x.Name == "Tomato Seed"), 3)
            };
            Enemy ant = new Enemy("Ant", 3, new Tuple<int, int>(0, 1), -60,
                1, new Tuple<int, int>(0, 0), antLoot);

            List<Tuple<Item, int>> bugLoot = new List<Tuple<Item, int>> 
            {
                new Tuple<Item, int>(items.Find(x => x.Name == "Grain of Rice"), 2),
                new Tuple<Item, int>(items.Find(x => x.Name == "Tomato Seed"), 3),
                new Tuple<Item, int>(items.Find(x => x.Name == "Pumpkin Seed"), 4)
            };
            Enemy bug = new Enemy("Bug", 5, new Tuple<int, int>(0, 1), -60,
                2, new Tuple<int, int>(0, 0), bugLoot);

            List<Tuple<Item, int>> spiderLoot = new List<Tuple<Item, int>>
            {
                new Tuple<Item, int>(items.Find(x => x.Name == "Spider Web"), 1),
                new Tuple<Item, int>(items.Find(x => x.Name == "Spider Leg"), 1),

                new Tuple<Item, int>(items.Find(x => x.Name == "Tomato Seed"), 2),
                new Tuple<Item, int>(items.Find(x => x.Name == "Pumpkin Seed"), 4),
                new Tuple<Item, int>(items.Find(x => x.Name == "Chia Seed"), 5)
            };
            Enemy spider = new Enemy("Spider", 8, new Tuple<int, int>(0, 1), -60,
                3, new Tuple<int, int>(0, 0), spiderLoot);

            List<Tuple<Item, int>> cockroachLoot = new List<Tuple<Item, int>>() 
            {
                new Tuple<Item, int>(items.Find(x => x.Name == "Pumpkin Seed"), 3),
                new Tuple<Item, int>(items.Find(x => x.Name == "Chia Seed"), 3)
            };
            Enemy cockroach = new Enemy("Cockroach", 9, new Tuple<int, int>(0, 2), -60,
                5, new Tuple<int, int>(0, 1), cockroachLoot);


            List<Tuple<Item, int>> mantisLoot = new List<Tuple<Item, int>>()
            {
                new Tuple<Item, int>(items.Find(x => x.Name == "Mantis Antennae"), 1),

                new Tuple<Item, int>(items.Find(x => x.Name == "Peanut"), 3),
                new Tuple<Item, int>(items.Find(x => x.Name == "Tobacco Leaf"), 5),
                new Tuple<Item, int>(items.Find(x => x.Name == "Wheat"), 10)
            };
            Enemy mantis = new Enemy("Mantis", 11, new Tuple<int, int>(1, 1), -50,
                6, new Tuple<int, int>(0, 1), mantisLoot);

            List<Tuple<Item, int>> frogLoot = new List<Tuple<Item, int>>()
            {
                new Tuple<Item, int>(items.Find(x => x.Name == "Frog Venom"), 1),

                new Tuple<Item, int>(items.Find(x => x.Name == "Peanut"), 3),
                new Tuple<Item, int>(items.Find(x => x.Name == "Walnut"), 3),
                 new Tuple<Item, int>(items.Find(x => x.Name == "Millet"), 6),

                new Tuple<Item, int>(items.Find(x => x.Name == "Wooden Stick"), 11),
                new Tuple<Item, int>(items.Find(x => x.Name == "Worn Leather Boots"), 15),
            };
            Enemy frog = new Enemy("Frog", 20, new Tuple<int, int>(1, 2), -50,
                8, new Tuple<int, int>(0, 2), frogLoot);

            List<Tuple<Item, int>> snakeLoot = new List<Tuple<Item, int>>()
            {
                new Tuple<Item, int>(items.Find(x => x.Name == "Snake Venom"), 1),

                new Tuple<Item, int>(items.Find(x => x.Name == "Walnut"), 3),
                new Tuple<Item, int>(items.Find(x => x.Name == "Tobacco Leaf"), 3),
                new Tuple<Item, int>(items.Find(x => x.Name == "Wheat"), 6),
                new Tuple<Item, int>(items.Find(x => x.Name == "Rice"), 6),

                new Tuple<Item, int>(items.Find(x => x.Name == "Wooden Stick"), 6),
                new Tuple<Item, int>(items.Find(x => x.Name == "Worn Leather Legs"), 8),
                new Tuple<Item, int>(items.Find(x => x.Name == "Worn Leather Armor"), 10),
                new Tuple<Item, int>(items.Find(x => x.Name == "Rock"), 13),
            };
            Enemy snake = new Enemy("Snake", 35, new Tuple<int, int>(1, 3), -50,
                13, new Tuple<int, int>(0, 4), snakeLoot);


            List<Tuple<Item, int>> blackWidowLoot = new List<Tuple<Item, int>>()
            {
                new Tuple<Item, int>(items.Find(x => x.Name == "Black Widow Venom"), 1),
                new Tuple<Item, int>(items.Find(x => x.Name == "Black Widow Leg"), 1),
            };
            Enemy blackWidow = new Enemy("Black Widow", 175, new Tuple<int, int>(2, 4), -30,
                250, new Tuple<int, int>(0, 0), blackWidowLoot, true);


            List<Tuple<Item, int>> bloodSpiderLoot = new List<Tuple<Item, int>>()
            {
                new Tuple<Item, int>(items.Find(x => x.Name == "Blood Spider Blood"), 1),

                new Tuple<Item, int>(items.Find(x => x.Name == "White Beans"), 4),
                new Tuple<Item, int>(items.Find(x => x.Name == "Kidney Beans"), 6),
                new Tuple<Item, int>(items.Find(x => x.Name == "Black Beans"), 14),

                new Tuple<Item, int>(items.Find(x => x.Name == "Worn Leather Helmet"), 6),
                new Tuple<Item, int>(items.Find(x => x.Name == "Worn Leather Armor"), 8),
            };
            Enemy bloodSpider = new Enemy("Blood Spider", 60, new Tuple<int, int>(1, 4), -40,
                25, new Tuple<int, int>(0, 7), bloodSpiderLoot);

            List<Tuple<Item, int>> tarantulaLoot = new List<Tuple<Item, int>>()
            {
                new Tuple<Item, int>(items.Find(x => x.Name == "Tarantula Egg"), 1),

                new Tuple<Item, int>(items.Find(x => x.Name == "White Beans"), 4),
                new Tuple<Item, int>(items.Find(x => x.Name == "Kidney Beans"), 5),
                new Tuple<Item, int>(items.Find(x => x.Name == "Black Beans"), 12),

                new Tuple<Item, int>(items.Find(x => x.Name == "Leather Boots"), 8),
                new Tuple<Item, int>(items.Find(x => x.Name == "Leather Helmet"), 8),
                new Tuple<Item, int>(items.Find(x => x.Name == "Leather Legs"), 10),              
                new Tuple<Item, int>(items.Find(x => x.Name == "Quilted Helmet"), 13),
            };
            Enemy tarantula = new Enemy("Tarantula", 90, new Tuple<int, int>(2, 5), -40,
                40, new Tuple<int, int>(0, 7), tarantulaLoot);

            List<Tuple<Item, int>> bananaSpiderLoot = new List<Tuple<Item, int>>()
            {
                new Tuple<Item, int>(items.Find(x => x.Name == "Banana"), 4),
                new Tuple<Item, int>(items.Find(x => x.Name == "Kidney Beans"), 4),
                new Tuple<Item, int>(items.Find(x => x.Name == "Black Beans"), 10),

                new Tuple<Item, int>(items.Find(x => x.Name == "Leather Armor"), 7),
                new Tuple<Item, int>(items.Find(x => x.Name == "Leather Legs"), 7),
                new Tuple<Item, int>(items.Find(x => x.Name == "Quilted Boots"), 8),
            };
            Enemy bananaSpider = new Enemy("Banana Spider", 125, new Tuple<int, int>(2, 5), -30,
                70, new Tuple<int, int>(5, 15), bananaSpiderLoot);

            List<Tuple<Item, int>> crabSpiderLoot = new List<Tuple<Item, int>>()
            {
                new Tuple<Item, int>(items.Find(x => x.Name == "Crab Spider Eye"), 1),

                new Tuple<Item, int>(items.Find(x => x.Name == "Crab Spider Leg"), 4),
                new Tuple<Item, int>(items.Find(x => x.Name == "White Beans"), 5),
                new Tuple<Item, int>(items.Find(x => x.Name == "Black Beans"), 6),

                new Tuple<Item, int>(items.Find(x => x.Name == "Quilted Boots"), 5),
                new Tuple<Item, int>(items.Find(x => x.Name == "Quilted Helmet"), 5),
            };
            Enemy crabSpider = new Enemy("Crab Spider", 95, new Tuple<int, int>(4, 12), -50,
                80, new Tuple<int, int>(0, 5), crabSpiderLoot);

            List<Tuple<Item, int>> orangeSpiderLoot = new List<Tuple<Item, int>>()
            {
                new Tuple<Item, int>(items.Find(x => x.Name == "Orange Blob"), 1),

                new Tuple<Item, int>(items.Find(x => x.Name == "Orange Spider Venom"), 2),
                new Tuple<Item, int>(items.Find(x => x.Name == "Black Beans"), 4),

                new Tuple<Item, int>(items.Find(x => x.Name == "Wooden Sword"), 5),
                new Tuple<Item, int>(items.Find(x => x.Name == "Quilted Legs"), 4)
            };
            Enemy orangeSpider = new Enemy("Orange Spider", 125, new Tuple<int, int>(2, 5), -15,
                100, new Tuple<int, int>(0, 8), orangeSpiderLoot);


            List<Tuple<Item, int>> squirrelLoot = new List<Tuple<Item, int>>()
            {
                new Tuple<Item, int>(items.Find(x => x.Name == "Tomato"), 6),
                new Tuple<Item, int>(items.Find(x => x.Name == "Cucumber"), 6),
                new Tuple<Item, int>(items.Find(x => x.Name == "Aubergine"), 6),
                new Tuple<Item, int>(items.Find(x => x.Name == "Courgette"), 9),

                new Tuple<Item, int>(items.Find(x => x.Name == "Worn Bronze Boots"), 7),               
                new Tuple<Item, int>(items.Find(x => x.Name == "Worn Bronze Helmet"), 10),        
            };
            Enemy squirrel = new Enemy("Squirrel", 200, new Tuple<int, int>(3, 6), 0,
                160, new Tuple<int, int>(5, 15), squirrelLoot);

            List<Tuple<Item, int>> owlLoot = new List<Tuple<Item, int>>()
            {
                new Tuple<Item, int>(items.Find(x => x.Name == "Tomato"), 5),
                new Tuple<Item, int>(items.Find(x => x.Name == "Cucumber"), 5),
                new Tuple<Item, int>(items.Find(x => x.Name == "Aubergine"), 5),
                new Tuple<Item, int>(items.Find(x => x.Name == "Courgette"), 10),

                new Tuple<Item, int>(items.Find(x => x.Name == "Worn Bronze Boots"), 7),
                new Tuple<Item, int>(items.Find(x => x.Name == "Worn Bronze Helmet"), 10),
                new Tuple<Item, int>(items.Find(x => x.Name == "Worn Bronze Legs"), 12),
            };
            Enemy owl = new Enemy("Owl", 150, new Tuple<int, int>(0, 12), 20,
                200, new Tuple<int, int>(5, 25), squirrelLoot);

            List<Tuple<Item, int>> hedgeHogLoot = new List<Tuple<Item, int>>()
            {
                new Tuple<Item, int>(items.Find(x => x.Name == "Apple"), 5),
                new Tuple<Item, int>(items.Find(x => x.Name == "Pear"), 5),
                new Tuple<Item, int>(items.Find(x => x.Name == "Truffles"), 6),

                new Tuple<Item, int>(items.Find(x => x.Name == "Worn Bronze Boots"), 6),
                new Tuple<Item, int>(items.Find(x => x.Name == "Worn Bronze Helmet"), 6),
                new Tuple<Item, int>(items.Find(x => x.Name == "Worn Bronze Legs"), 11),
                new Tuple<Item, int>(items.Find(x => x.Name == "Wooden Well Carved Sword"), 11),
            };
            Enemy hedgeHog = new Enemy("Hedge Hog", 250, new Tuple<int, int>(1, 7), 75,
                300, new Tuple<int, int>(2, 10), hedgeHogLoot);

            List<Tuple<Item, int>> foxLoot = new List<Tuple<Item, int>>()
            {
                new Tuple<Item, int>(items.Find(x => x.Name == "Fox Tail"), 1),

                new Tuple<Item, int>(items.Find(x => x.Name == "Aubergine"), 4),
                new Tuple<Item, int>(items.Find(x => x.Name == "Courgette"), 4),
                new Tuple<Item, int>(items.Find(x => x.Name == "Truffles"), 5),

                new Tuple<Item, int>(items.Find(x => x.Name == "Worn Bronze Legs"), 5),
                new Tuple<Item, int>(items.Find(x => x.Name == "Worn Bronze Armor"), 8),
                new Tuple<Item, int>(items.Find(x => x.Name == "Wooden Well Carved Sword"), 6),
            };
            Enemy fox = new Enemy("Fox", 250, new Tuple<int, int>(4, 13), 0,
                400, new Tuple<int, int>(10, 20), foxLoot);

            List<Tuple<Item, int>> wolfLoot = new List<Tuple<Item, int>>()
            {
                new Tuple<Item, int>(items.Find(x => x.Name == "Wolf Paw"), 1),

                new Tuple<Item, int>(items.Find(x => x.Name == "Aubergine"), 4),
                new Tuple<Item, int>(items.Find(x => x.Name == "Pear"), 5),
                new Tuple<Item, int>(items.Find(x => x.Name == "Truffles"), 5),

                new Tuple<Item, int>(items.Find(x => x.Name == "Worn Bronze Legs"), 2),
                new Tuple<Item, int>(items.Find(x => x.Name == "Worn Bronze Armor"), 3),
                new Tuple<Item, int>(items.Find(x => x.Name == "Used Bronze Sword"), 5),
            };
            Enemy wolf = new Enemy("Wolf", 300, new Tuple<int, int>(6, 14), 20,
                600, new Tuple<int, int>(10, 25), wolfLoot);

            List<Tuple<Item, int>> witchLoot = new List<Tuple<Item, int>>()
            {
                new Tuple<Item, int>(items.Find(x => x.Name == "Bronze Axe"), 1)
            };
            Enemy witch = new Enemy("Witch", 1750, new Tuple<int, int>(0, 40), 25,
                4000, new Tuple<int, int>(0, 0), witchLoot, true);
            #endregion

            #region Amfurce
            List<Tuple<Item, int>> baconThoLoot = new List<Tuple<Item, int>>()
            {
                new Tuple<Item, int>(items.Find(x => x.Name == "Apple"), 3),
                new Tuple<Item, int>(items.Find(x => x.Name == "Aubergine"), 3),
                new Tuple<Item, int>(items.Find(x => x.Name == "Courgette"), 4),
                new Tuple<Item, int>(items.Find(x => x.Name == "Pear"), 4),
                new Tuple<Item, int>(items.Find(x => x.Name == "Truffles"), 4),
            };
            Enemy baconTho = new Enemy("Bacon Tho", 700, new Tuple<int, int>(5, 20), 0,
                650, new Tuple<int, int>(0, 25), baconThoLoot);

            List<Tuple<Item, int>> plantFeelingsLoot = new List<Tuple<Item, int>>()
            {
                new Tuple<Item, int>(items.Find(x => x.Name == "Pear"), 4),
                new Tuple<Item, int>(items.Find(x => x.Name == "Vegan Cheese Slice"), 4),
                new Tuple<Item, int>(items.Find(x => x.Name == "Vegan Slice of Ham"), 5),
                new Tuple<Item, int>(items.Find(x => x.Name == "Roll"), 5),
            };
            Enemy plantFeelings = new Enemy("Plant Feelings", 600, new Tuple<int, int>(10, 25), 10,
                800, new Tuple<int, int>(0, 30), plantFeelingsLoot);

            List<Tuple<Item, int>> proteinDeficiencyLoot = new List<Tuple<Item, int>>()
            {
                new Tuple<Item, int>(items.Find(x => x.Name == "Vegan Cheese Slice"), 4),
                new Tuple<Item, int>(items.Find(x => x.Name == "Roll"), 5),
                new Tuple<Item, int>(items.Find(x => x.Name == "Vegan Butter"), 5),
                new Tuple<Item, int>(items.Find(x => x.Name == "Salt"), 5),
                new Tuple<Item, int>(items.Find(x => x.Name == "Pepper"), 5),
            };
            Enemy proteinDeficiency = new Enemy("Protein Deficiency", 800, new Tuple<int, int>(10, 20), 40,
                1000, new Tuple<int, int>(0, 40), proteinDeficiencyLoot);

            Enemy pastYou = new Enemy("Past You", 1000, new Tuple<int, int>(10, 10), 0,
                1000, new Tuple<int, int>(0, 0), new List<Tuple<Item, int>>(), true);

            List<Tuple<Item, int>> viciousMeatEaterLoot = new List<Tuple<Item, int>>()
            {
                new Tuple<Item, int>(items.Find(x => x.Name == "Tempeh"), 2),

                new Tuple<Item, int>(items.Find(x => x.Name == "Vegan Cheese Slice"), 5),
                new Tuple<Item, int>(items.Find(x => x.Name == "Vegan Slice of Ham"), 5),
                new Tuple<Item, int>(items.Find(x => x.Name == "Roll"), 5),
                new Tuple<Item, int>(items.Find(x => x.Name == "Vegan Butter"), 5),

                new Tuple<Item, int>(items.Find(x => x.Name == "Cruelty-Free Leather Boots"), 18),
                new Tuple<Item, int>(items.Find(x => x.Name == "Cruelty-Free Leather Helmet"), 28),
                new Tuple<Item, int>(items.Find(x => x.Name == "Cruelty-Free Leather Legs"), 40)
            };
            Enemy viciousMeatEater = new Enemy("Vicious Meat Eater", 750, new Tuple<int, int>(20, 40), -20,
                700, new Tuple<int, int>(20, 50), viciousMeatEaterLoot);

            List<Tuple<Item, int>> sharkFinLoverLoot = new List<Tuple<Item, int>>()
            {
                new Tuple<Item, int>(items.Find(x => x.Name == "Tempeh"), 2),

                new Tuple<Item, int>(items.Find(x => x.Name == "Vegan Cheese Slice"), 4),
                new Tuple<Item, int>(items.Find(x => x.Name == "Vegan Slice of Ham"), 4),
                new Tuple<Item, int>(items.Find(x => x.Name == "Roll"), 4),
                new Tuple<Item, int>(items.Find(x => x.Name == "Vegan Butter"), 4),

                new Tuple<Item, int>(items.Find(x => x.Name == "Cruelty-Free Leather Boots"), 15),
                new Tuple<Item, int>(items.Find(x => x.Name == "Cruelty-Free Leather Helmet"), 24),
                new Tuple<Item, int>(items.Find(x => x.Name == "Cruelty-Free Leather Legs"), 36),
                new Tuple<Item, int>(items.Find(x => x.Name == "Cruelty-Free Leather Armor"), 36),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Wooden Sword"), 40)
            };
            Enemy sharkFinLover = new Enemy("Shark Fin Lover", 900, new Tuple<int, int>(30, 50), -10,
                900, new Tuple<int, int>(30, 60), sharkFinLoverLoot);

            List<Tuple<Item, int>> obeseHamburgerEaterLoot = new List<Tuple<Item, int>>()
            {
                new Tuple<Item, int>(items.Find(x => x.Name == "Tempeh"), 2),

                new Tuple<Item, int>(items.Find(x => x.Name == "Vegan Cheese Slice"), 5),
                new Tuple<Item, int>(items.Find(x => x.Name == "Vegan Slice of Ham"), 5),
                new Tuple<Item, int>(items.Find(x => x.Name == "Roll"), 5),
                new Tuple<Item, int>(items.Find(x => x.Name == "Vegan Butter"), 5),
                new Tuple<Item, int>(items.Find(x => x.Name == "Orange"), 7),
                new Tuple<Item, int>(items.Find(x => x.Name == "Grape"), 7),

                new Tuple<Item, int>(items.Find(x => x.Name == "Cruelty-Free Leather Boots"), 12),
                new Tuple<Item, int>(items.Find(x => x.Name == "Cruelty-Free Leather Helmet"), 20),
                new Tuple<Item, int>(items.Find(x => x.Name == "Cruelty-Free Leather Legs"), 30),
                new Tuple<Item, int>(items.Find(x => x.Name == "Cruelty-Free Leather Armor"), 30),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Wooden Sword"), 30),
            };
            Enemy obeseHamburgerEater = new Enemy("Obese Hamburger Eater", 1300, new Tuple<int, int>(30, 55), -20,
                1100, new Tuple<int, int>(30, 60), obeseHamburgerEaterLoot);

            List<Tuple<Item, int>> patientWithHypertensionLoot = new List<Tuple<Item, int>>()
            {
                new Tuple<Item, int>(items.Find(x => x.Name == "Pak Choi"), 2),

                new Tuple<Item, int>(items.Find(x => x.Name == "Orange"), 5),
                new Tuple<Item, int>(items.Find(x => x.Name == "Grape"), 5),
                new Tuple<Item, int>(items.Find(x => x.Name == "Tofu"), 6),
                new Tuple<Item, int>(items.Find(x => x.Name == "Strawberry"), 6),
                new Tuple<Item, int>(items.Find(x => x.Name == "Raspberry"), 6),

                new Tuple<Item, int>(items.Find(x => x.Name == "Cruelty-Free Leather Boots"), 12),
                new Tuple<Item, int>(items.Find(x => x.Name == "Cruelty-Free Leather Helmet"), 22),
                new Tuple<Item, int>(items.Find(x => x.Name == "Cruelty-Free Leather Legs"), 28),
                new Tuple<Item, int>(items.Find(x => x.Name == "Cruelty-Free Leather Armor"), 28),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Wooden Sword"), 28),
            };
            Enemy patienWithHypertension = new Enemy("Patient with Hypertension", 1100, new Tuple<int, int>(45, 45), 0,
                1000, new Tuple<int, int>(30, 50), patientWithHypertensionLoot);

            List<Tuple<Item, int>> madmanLoot = new List<Tuple<Item, int>>()
            {
                new Tuple<Item, int>(items.Find(x => x.Name == "Pak Choi"), 2),

                new Tuple<Item, int>(items.Find(x => x.Name == "Mushrooms"), 2),
                new Tuple<Item, int>(items.Find(x => x.Name == "Orange"), 4),
                new Tuple<Item, int>(items.Find(x => x.Name == "Grape"), 4),
                new Tuple<Item, int>(items.Find(x => x.Name == "Tofu"), 6),
                new Tuple<Item, int>(items.Find(x => x.Name == "Strawberry"), 6),
                new Tuple<Item, int>(items.Find(x => x.Name == "Raspberry"), 6),

                new Tuple<Item, int>(items.Find(x => x.Name == "Cruelty-Free Leather Boots"), 10),
                new Tuple<Item, int>(items.Find(x => x.Name == "Cruelty-Free Leather Helmet"), 14),
                new Tuple<Item, int>(items.Find(x => x.Name == "Cruelty-Free Leather Legs"), 16),
                new Tuple<Item, int>(items.Find(x => x.Name == "Cruelty-Free Leather Armor"), 16),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Wooden Sword"), 16),
            };
            Enemy madman = new Enemy("Madman", 800, new Tuple<int, int>(0, 130), -10,
                1400, new Tuple<int, int>(0, 100), madmanLoot);

            List<Tuple<Item, int>> cancerPatientLoot = new List<Tuple<Item, int>>()
            {
                new Tuple<Item, int>(items.Find(x => x.Name == "Pak Choi"), 2),

                new Tuple<Item, int>(items.Find(x => x.Name == "Orange"), 3),
                new Tuple<Item, int>(items.Find(x => x.Name == "Grape"), 3),
                new Tuple<Item, int>(items.Find(x => x.Name == "Tofu"), 3),
                new Tuple<Item, int>(items.Find(x => x.Name == "Strawberry"), 5),
                new Tuple<Item, int>(items.Find(x => x.Name == "Raspberry"), 5),

                new Tuple<Item, int>(items.Find(x => x.Name == "Cruelty-Free Leather Legs"), 11),
                new Tuple<Item, int>(items.Find(x => x.Name == "Cruelty-Free Leather Armor"), 11),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Wooden Sword"), 15),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Leather Boots"), 20),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Leather Helmet"), 25)              
            };
            Enemy cancerPatient = new Enemy("Cancer Patient", 2000, new Tuple<int, int>(40, 60), 20,
                2000, new Tuple<int, int>(30, 70), cancerPatientLoot);

            List<Tuple<Item, int>> butcherLoot = new List<Tuple<Item, int>>()
            {
                new Tuple<Item, int>(items.Find(x => x.Name == "Soy Sauce"), 3),
                new Tuple<Item, int>(items.Find(x => x.Name == "Sesame Oil"), 3),

                new Tuple<Item, int>(items.Find(x => x.Name == "Tofu"), 4),
                new Tuple<Item, int>(items.Find(x => x.Name == "Strawberry"), 4),
                new Tuple<Item, int>(items.Find(x => x.Name == "Raspberry"), 4),
                new Tuple<Item, int>(items.Find(x => x.Name == "Blueberry"), 4),
                new Tuple<Item, int>(items.Find(x => x.Name == "Smoked Paprika"), 6),

                new Tuple<Item, int>(items.Find(x => x.Name == "Cruelty-Free Leather Armor"), 6),            
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Leather Boots"), 9),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Leather Helmet"), 12),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Leather Legs"), 15),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Leather Armor"), 20),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Wooden Axe"), 30)
            };
            Enemy butcher = new Enemy("Butcher", 1500, new Tuple<int, int>(30, 60), 0,
                1300, new Tuple<int, int>(40, 90), butcherLoot);

            List<Tuple<Item, int>> fishermanLoot = new List<Tuple<Item, int>>()
            {
                new Tuple<Item, int>(items.Find(x => x.Name == "Soy Sauce"), 3),
                new Tuple<Item, int>(items.Find(x => x.Name == "Sesame Oil"), 3),

                new Tuple<Item, int>(items.Find(x => x.Name == "Strawberry"), 3),
                new Tuple<Item, int>(items.Find(x => x.Name == "Raspberry"), 4),
                new Tuple<Item, int>(items.Find(x => x.Name == "Blueberry"), 4),
                new Tuple<Item, int>(items.Find(x => x.Name == "Smoked Paprika"), 5),
                new Tuple<Item, int>(items.Find(x => x.Name == "Soy Milk"), 7),
                new Tuple<Item, int>(items.Find(x => x.Name == "Rice Milk"), 7),

                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Leather Boots"), 8),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Leather Helmet"), 11),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Leather Legs"), 14),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Leather Armor"), 18),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Wooden Axe"), 25)
            };
            Enemy fisherman = new Enemy("Fisherman", 2000, new Tuple<int, int>(35, 65), 0,
                1800, new Tuple<int, int>(30, 100), fishermanLoot);

            List<Tuple<Item, int>> bullfighterLoot = new List<Tuple<Item, int>>()
            {
                new Tuple<Item, int>(items.Find(x => x.Name == "Soy Sauce"), 3),
                new Tuple<Item, int>(items.Find(x => x.Name == "Sesame Oil"), 3),

                new Tuple<Item, int>(items.Find(x => x.Name == "Soy Milk"), 3),
                new Tuple<Item, int>(items.Find(x => x.Name == "Rice Milk"), 3),
                new Tuple<Item, int>(items.Find(x => x.Name == "Oat Milk"), 4),

                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Leather Boots"), 6),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Leather Helmet"), 8),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Leather Legs"), 8),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Leather Armor"), 11),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Wooden Axe"), 16)
            };
            Enemy bullfighter = new Enemy("Bullfighter", 1600, new Tuple<int, int>(40, 85), 10,
                2400, new Tuple<int, int>(20, 120), bullfighterLoot);

            List<Tuple<Item, int>> cowInseminatorLoot = new List<Tuple<Item, int>>()
            {
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Leather"), 4),

                new Tuple<Item, int>(items.Find(x => x.Name == "Soy Milk"), 5),
                new Tuple<Item, int>(items.Find(x => x.Name == "Rice Milk"), 5),
                new Tuple<Item, int>(items.Find(x => x.Name == "Oat Milk"), 5),
                new Tuple<Item, int>(items.Find(x => x.Name == "Cashew Milk"), 6),
                new Tuple<Item, int>(items.Find(x => x.Name == "Almond Milk"), 6),

                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Leather Boots"), 10),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Leather Helmet"), 10),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Leather Legs"), 17),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Leather Armor"), 21),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Wooden Axe"), 20)
            };
            Enemy cowInseminator = new Enemy("Cow Inseminator", 1100, new Tuple<int, int>(110, 140), 0,
                2300, new Tuple<int, int>(40, 60), cowInseminatorLoot);

            List<Tuple<Item, int>> beekeeperLoot = new List<Tuple<Item, int>>()
            {
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Leather"), 3),

                new Tuple<Item, int>(items.Find(x => x.Name == "Soy Milk"), 4),
                new Tuple<Item, int>(items.Find(x => x.Name == "Rice Milk"), 4),
                new Tuple<Item, int>(items.Find(x => x.Name == "Oat Milk"), 4),
                new Tuple<Item, int>(items.Find(x => x.Name == "Cashew Milk"), 5),
                new Tuple<Item, int>(items.Find(x => x.Name == "Almond Milk"), 5),

                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Leather Legs"), 9),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Leather Armor"), 9),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Wooden Axe"), 9),
                new Tuple<Item, int>(items.Find(x => x.Name == "Empathy Boots"), 18),
            };
            Enemy beekeeper = new Enemy("Beekeeper", 1500, new Tuple<int, int>(80, 140), 25,
                2700, new Tuple<int, int>(50, 100), beekeeperLoot);

            List<Tuple<Item, int>> factoryFarmerLoot = new List<Tuple<Item, int>>()
            {
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Leather"), 3),

                new Tuple<Item, int>(items.Find(x => x.Name == "Soy Milk"), 3),
                new Tuple<Item, int>(items.Find(x => x.Name == "Rice Milk"), 3),
                new Tuple<Item, int>(items.Find(x => x.Name == "Oat Milk"), 3),
                new Tuple<Item, int>(items.Find(x => x.Name == "Cashew Milk"), 4),
                new Tuple<Item, int>(items.Find(x => x.Name == "Almond Milk"), 5),

                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Leather Legs"), 6),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Leather Armor"), 6),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Wooden Axe"), 9),
                new Tuple<Item, int>(items.Find(x => x.Name == "Empathy Boots"), 18),
            };
            Enemy factoryFarmer = new Enemy("Factory Farmer", 2500, new Tuple<int, int>(50, 100), 75,
                3500, new Tuple<int, int>(30, 150), factoryFarmerLoot);

            List<Tuple<Item, int>> zooWorkerLoot = new List<Tuple<Item, int>>()
            {
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Leather"), 3),

                new Tuple<Item, int>(items.Find(x => x.Name == "Cashew Milk"), 3),
                new Tuple<Item, int>(items.Find(x => x.Name == "Almond Milk"), 3),
                new Tuple<Item, int>(items.Find(x => x.Name == "Seitan"), 4),

                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Leather Legs"), 5),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Leather Armor"), 5),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Wooden Axe"), 9),
                new Tuple<Item, int>(items.Find(x => x.Name == "Empathy Boots"), 10),
                new Tuple<Item, int>(items.Find(x => x.Name == "Empathy Helmet"), 10),
            };
            Enemy zooWorker = new Enemy("Zoo Worker", 2000, new Tuple<int, int>(110, 170), 50,
                4500, new Tuple<int, int>(0, 50), beekeeperLoot);

            Enemy hunter = new Enemy("Hunter", 4000, new Tuple<int, int>(100, 200), 75,
                7000, new Tuple<int, int>(0, 0), new List<Tuple<Item, int>>());

            Enemy poacher = new Enemy("Poacher", 6000, new Tuple<int, int>(100, 125), 125,
                8000, new Tuple<int, int>(0, 0), new List<Tuple<Item, int>>());

            Enemy slaughterhouseOwner = new Enemy("Slaughterhouse Owner", 7000, new Tuple<int, int>(100, 200), 75,
                10000, new Tuple<int, int>(0, 0), new List<Tuple<Item, int>>());

            List<Tuple<Item, int>> angryMeatLoverLoot = new List<Tuple<Item, int>>()
            {
                new Tuple<Item, int>(items.Find(x => x.Name == "Fragment of Recipe"), 2),

                new Tuple<Item, int>(items.Find(x => x.Name == "Cashew Milk"), 4),
                new Tuple<Item, int>(items.Find(x => x.Name == "Almond Milk"), 4),
                new Tuple<Item, int>(items.Find(x => x.Name == "Seitan"), 5),
                new Tuple<Item, int>(items.Find(x => x.Name == "Soy Protein"), 5),

                new Tuple<Item, int>(items.Find(x => x.Name == "Empathy Armor"), 4),
                new Tuple<Item, int>(items.Find(x => x.Name == "Empathy Legs"), 4),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Boots of Flowing Water"), 9),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Helmet of Flowing Water"), 10),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Legs of Flowing Water"), 12),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Armor of Flowing Water"), 15),
            };
            Enemy angryMeatLover = new Enemy("Angry Meat Lover", 2000, new Tuple<int, int>(150, 200), 100,
                6000, new Tuple<int, int>(50, 250), angryMeatLoverLoot);

            List<Tuple<Item, int>> foriousEggEaterLoot = new List<Tuple<Item, int>>()
            {
                new Tuple<Item, int>(items.Find(x => x.Name == "Fragment of Recipe"), 2),

                new Tuple<Item, int>(items.Find(x => x.Name == "Cashew Milk"), 3),
                new Tuple<Item, int>(items.Find(x => x.Name == "Almond Milk"), 3),
                new Tuple<Item, int>(items.Find(x => x.Name == "Seitan"), 3),
                new Tuple<Item, int>(items.Find(x => x.Name == "Soy Protein"), 3),

                new Tuple<Item, int>(items.Find(x => x.Name == "Empathy Armor"), 4),
                new Tuple<Item, int>(items.Find(x => x.Name == "Empathy Legs"), 4),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Boots of Flowing Water"), 8),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Helmet of Flowing Water"), 9),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Legs of Flowing Water"), 11),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Armor of Flowing Water"), 13),
            };
            Enemy foriousEggEater = new Enemy("Forious Egg Eater", 2000, new Tuple<int, int>(200, 250), 125,
                8000, new Tuple<int, int>(50, 250), foriousEggEaterLoot);

            List<Tuple<Item, int>> desperatedBaconLoverLoot = new List<Tuple<Item, int>>()
            {
                new Tuple<Item, int>(items.Find(x => x.Name == "Fragment of Recipe"), 2),

                new Tuple<Item, int>(items.Find(x => x.Name == "Cashew Milk"), 3),
                new Tuple<Item, int>(items.Find(x => x.Name == "Almond Milk"), 3),
                new Tuple<Item, int>(items.Find(x => x.Name == "Seitan"), 3),
                new Tuple<Item, int>(items.Find(x => x.Name == "Soy Protein"), 3),

                new Tuple<Item, int>(items.Find(x => x.Name == "Empathy Armor"), 3),
                new Tuple<Item, int>(items.Find(x => x.Name == "Empathy Legs"), 3),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Boots of Flowing Water"), 6),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Helmet of Flowing Water"), 7),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Legs of Flowing Water"), 8),
                new Tuple<Item, int>(items.Find(x => x.Name == "BIO Armor of Flowing Water"), 9),
            };
            Enemy desperatedBaconLover = new Enemy("Desperated Bacon Lover", 2500, new Tuple<int, int>(200, 300), 125,
                8000, new Tuple<int, int>(50, 250), desperatedBaconLoverLoot);

            Enemy ruinedKing = new Enemy("Ruined King", 50000, new Tuple<int, int>(500, 1000), 250,
                50000, new Tuple<int, int>(0, 0), new List<Tuple<Item, int>>(), true);
            #endregion

            enemies = new List<Enemy>()
            {
                #region Farm
                ant,
                bug,
                spider,

                cockroach,

                mantis,
                frog,
                snake,

                blackWidow,

                bloodSpider,
                tarantula,
                bananaSpider,
                crabSpider,
                orangeSpider,
                
                squirrel,
                owl,
                hedgeHog,
                fox,
                wolf,

                witch,
                #endregion

                #region Amfurce
                baconTho,
                plantFeelings,
                proteinDeficiency,

                pastYou,

                viciousMeatEater,
                sharkFinLover,
                obeseHamburgerEater,

                patienWithHypertension,
                madman,
                cancerPatient,

                butcher,
                fisherman,
                bullfighter,

                cowInseminator,
                beekeeper,
                factoryFarmer,
                zooWorker,

                hunter,
                poacher,
                slaughterhouseOwner,

                angryMeatLover,
                foriousEggEater,
                desperatedBaconLover,

                ruinedKing
                #endregion
            };
        }

        void GenerateAbilities()
        {
            #region 0 - 11
            AbilityEffects luckyDayEffects = new AbilityEffects(0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 6, 0, 0, 0);
            Ability luckyDay = new Ability("Lucky Day", "@9|Enemy @15|gives up to @6|6 @15|more @6|gold",
                1, 3, 999, 2, luckyDayEffects);

            AbilityEffects strongHandEffects = new AbilityEffects(0, 0, 3, 5, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            Ability strongHand = new Ability("Strong Hand", "@15|Increases your @13|damage @15|range by @13|1 @15|to @13|5 @15|for 2 turns",
                2, 6, 2, 2, strongHandEffects);

            AbilityEffects doubleShotEffects = new AbilityEffects(0, 0, 0, 0, 1,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            Ability doubleShot = new Ability("Double Shot", "@15|Attack your @9|enemy @15|with @13|double @15|the power",
                4, 8, 1, 3, doubleShotEffects);

            AbilityEffects iKnowBetterEffects = new AbilityEffects(0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 2);
            Ability iKnowBetter = new Ability("I Know Better", "@9|Enemy @15|give @4|3 times @15|the @4|experience",
                4, 8, 999, 6, iKnowBetterEffects);

            AbilityEffects crippleEffects = new AbilityEffects(0, 0, 0, 0, 0,
                0, 0, -12, -24, 0, 0, 0, 0, 0, 0);
            Ability cripple = new Ability("Cripple", "@15|Decrease @9|enemy @13|damage @15|by @13|12 @15|to @13|24 @15|for 2 turns",
                5, 11, 2, 6, crippleEffects);

            AbilityEffects aegisEffects = new AbilityEffects(200, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            Ability aegis = new Ability("Aegis", "@15|Summon magical shield, giving yourself @5|200 defense @15|for 5 turns",
                7, 13, 5, 6, aegisEffects);

            AbilityEffects bowEffects = new AbilityEffects(0, 0, 0, 100, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            Ability bow = new Ability("Bow", "@15|Shoot @9|enemy @15|using bow, dealing up to @13|100 damage",
                8, 13, 1, 10, bowEffects);

            AbilityEffects breakTheirMindEffects = new AbilityEffects(0, 0, 0, 0, 0,
                -40, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            Ability breakTheirMind = new Ability("Break Their Mind", "@15|Make @9|enemy @15|mad, lowering their @5|defense @15|by @5|40 @15|for 6 turns",
                8, 13, 6, 12, breakTheirMindEffects);            

            AbilityEffects thunderLordEffects = new AbilityEffects(0, 0, 100, 100, 0,
               0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            Ability thunderLord = new Ability("Thunder Lord", "@15|Smite your @9|enemy@15|, dealing @13|100 damage",
                11, 14, 1, 15, thunderLordEffects);

            AbilityEffects rainEffects = new AbilityEffects(0, 0, 0, 0, 0,
               0, 0, 0, 0, 0, 0, 0, 4, 0, 0);
            Ability rain = new Ability("Rain", "@9|Enemy @15|give @6|5 times @15|more @6|gold",
                11, 15, 999, 15, rainEffects);

            AbilityEffects newToTheGameEffects = new AbilityEffects(0, 0, 0, 0, 0,
               0, 0, 0, 0, 0, 0, 0, 0, 500, 0);
            Ability newToTheGame = new Ability("New To The Game", "@9|Enemy @15|gives @4|500 @15|more @4|experience",
                11, 15, 999, 15, newToTheGameEffects);
            #endregion

            #region 13 - 19
            AbilityEffects godBlessingEffects = new AbilityEffects(0, 4, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            Ability godBlessing = new Ability("God Blessing", "@15|Pray to the God, increasing your @5|defense 5 times @15|for 7 turns",
                13, 20, 7, 20, godBlessingEffects);

            AbilityEffects hitOrMissEffects = new AbilityEffects(0, 0, 0, 0, 0,
               0, 0, -50, -50, 0, 0, 0, 0, 0, 0);
            Ability hitOrMiss = new Ability("Hit or Miss", "@9|Enemy @15|does @13|50 @15|less @13|damage @15|in 5 turns",
                15, 21, 5, 20, hitOrMissEffects);

            AbilityEffects learningExperienceEffects = new AbilityEffects(0, 0, 0, 0, 0,
               0, 0, 0, 0, 0, 0, 0, 0, 0, 1);
            Ability learningExperience = new Ability("Learning Experience", "@9|Enemy @15|gives @4|2 times @15|the @4|experience",
                15, 21, 999, 25, learningExperienceEffects);

            AbilityEffects defenseLessEffects = new AbilityEffects(0, 0, 0, 0, 0,
               -65, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            Ability defenseLess = new Ability("Defense Less", "@15|Destroy @9|enemy @5|defense@15|, lowering it by @5|65 @15|for 4 turns",
                15, 21, 4, 30, defenseLessEffects);

            AbilityEffects squareEffects = new AbilityEffects(0, 0, 0, 0, 2,
               0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            Ability square = new Ability("Square", "@15|Hit @9|enemy @13|3 times @15|in one turn",
                15, 25, 1, 30, squareEffects);

            AbilityEffects doubleEdgeSwordEffects = new AbilityEffects(0, 0, 0, 0, 3,
               0, 0, 0, 0, 3, 0, 0, 0, 0, 0);
            Ability doubleEdgeSword = new Ability("Double Edge Sword", "@15|Both @9|enemy @15|and you have @13|damage @15|multiplied by @13|4 @15|for 3 turns",
                19, 25, 3, 40, doubleEdgeSwordEffects);

            AbilityEffects berserkEffects = new AbilityEffects(-150, 0, 20, 200, 0,
               0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            Ability berserk = new Ability("Berserk", "@15|Lower your @5|defense @15|by @5|-150 @15|and increase your @13|damage @15|by @13|20 @15|to @13|200 @15|for 4 turns",
                19, 25, 4, 40, berserkEffects);
            #endregion

            #region 22 - 50
            AbilityEffects tortoiseEffects = new AbilityEffects(0, 5, 0, 0, 0,
               50, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            Ability tortoise = new Ability("Tortoise",
                "@15|Increases your @5|defense 6 times @15|and @9|enemy @5|defense @15|by @5|50 @15|for 10 turns",
                22, 29, 10, 50, tortoiseEffects);

            AbilityEffects betterVersionEffects = new AbilityEffects(0, 0, 0, 0, 0,
               0, 0, 0, 0, 0, 0, 0, 2, 0, 2);
            Ability betterVersion = new Ability("Better Version",
                "@9|Enemy @15|gives @6|3 times @15|the @6|gold @15|and @4|experience",
                22, 29, 999, 75, betterVersionEffects);

            AbilityEffects fasterThanEverEffects = new AbilityEffects(0, 0, 0, 0, 11,
               0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            Ability fasterThanEver = new Ability("Faster Than Ever",
                "@15|Hit @9|enemy @13|12 times @15|in one turn",
                26, 50, 1, 80, fasterThanEverEffects);

            AbilityEffects blindEffects = new AbilityEffects(0, 0, 0, 0, 0,
               0, 0, -400, 0, 0, 0, 0, 0, 0, 0);
            Ability blind = new Ability("Blind",
                "@15|Blind @9|enemy@15|, decreasing their @13|minimum damage @15|by @13|400 @15|for 4 turns",
                26, 50, 4, 80, blindEffects);

            AbilityEffects recoilEffects = new AbilityEffects(0, 0, -200, 400, 0,
                 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            Ability recoil = new Ability("Recoil",
                "@15|Increase your @13|maximum damage @15|by @13|400@15|, but decrease @13|minimum @15|by @13|200 @15|for 3 turns",
                30, 50, 3, 100, recoilEffects);

            AbilityEffects strongerThanYouThinkEffects = new AbilityEffects(0, 0.5, 0, 0, 0,
               0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            Ability strongerThanYouThink = new Ability("Stronger Than You Think",
                "@15|Increase your @5|defense @15|by @5|50% @15|till the end of the fight",
                32, 50, 999, 125, strongerThanYouThinkEffects);

            AbilityEffects littleBigDamageEffects = new AbilityEffects(0, 0, 0, 0, 0.3,
               0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            Ability littleBigDamage = new Ability("Little Big Damage",
                "@15|Increase your @13|damage @15|by @13|30% @15|till the end of the fight",
                32, 50, 999, 125, littleBigDamageEffects);
            #endregion

            abilities = new List<Ability>()
            {
                #region 0 - 11         
                luckyDay,
                strongHand,
                doubleShot,
                iKnowBetter,
                cripple,

                aegis,
                bow,
                breakTheirMind,
                #endregion

                #region 13 - 19
                thunderLord,
                rain,
                newToTheGame,
                godBlessing,

                hitOrMiss,
                learningExperience,
                defenseLess,          
                square,
                doubleEdgeSword,
                berserk,
                #endregion

                #region 22 - 50
                tortoise,
                betterVersion,

                fasterThanEver,
                blind,
                recoil,
                strongerThanYouThink,
                littleBigDamage,       
                #endregion
            };
        }

        void GenerateQuestItems()
        {
            #region Farm
            QuestItem spiderWeb = new QuestItem("Spider Web");

            QuestItem snakeVenom = new QuestItem("Snake Venom");
            QuestItem frogVenom = new QuestItem("Frog Venom");

            QuestItem tarantulaEgg = new QuestItem("Tarantula Egg");
            QuestItem crabSpiderEye = new QuestItem("Crab Spider Eye");
            QuestItem orangeBlob = new QuestItem("Orange Blob");

            QuestItem spiderLeg = new QuestItem("Spider Leg");
            QuestItem mantisAntennae = new QuestItem("Mantis Antennae");
            QuestItem bloodSpiderBlood = new QuestItem("Blood Spider Blood");
            QuestItem foxTail = new QuestItem("Fox Tail");
            QuestItem wolfPaw = new QuestItem("Wolf Paw");
            #endregion

            #region Amfurce
            QuestItem tempeh = new QuestItem("Tempeh");
            QuestItem pakChoi = new QuestItem("Pak Choi");
            QuestItem soySauce = new QuestItem("Soy Sauce");
            QuestItem sesameOil = new QuestItem("Sesame Oil");

            QuestItem bioLeather = new QuestItem("BIO Leather");
            QuestItem fragmentOfRecipe = new QuestItem("Fragment of Recipe");
            #endregion

            questItems = new List<QuestItem>()
            {
                #region Farm
                spiderWeb,
                snakeVenom,
                frogVenom,

                tarantulaEgg,
                crabSpiderEye,
                orangeBlob,

                spiderLeg,
                mantisAntennae,
                bloodSpiderBlood,
                foxTail,
                wolfPaw,
                #endregion

                #region Amfurce
                tempeh,
                pakChoi,
                soySauce,
                sesameOil,

                bioLeather,
                fragmentOfRecipe
                #endregion
            };
        }

        void GenerateQuests()
        {
            #region Farm
            List<Tuple<Enemy, int>> emptyEnemies = new List<Tuple<Enemy, int>>();
            List<Tuple<QuestItem, int>> emptyQuestItems = new List<Tuple<QuestItem, int>>();
            List<Item> emptyRewards = new List<Item>();

            Quest peterProblem = new Quest("Peter Problem", 
                "@15|Ask @9|Peter @15|about the @10|place",
                emptyEnemies, emptyQuestItems, 0, emptyRewards, true);

            List<Tuple<Enemy, int>> antVacuumEnemies = new List<Tuple<Enemy, int>>
            {
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Ant"), 6)
            };
            Quest antVacuum = new Quest("Ant Vacuum", 
                "I would give you vacuum cleaner, if I would have one",
                antVacuumEnemies, emptyQuestItems,
                10, emptyRewards);

            List<Tuple<QuestItem, int>> inTheWebQuestItems = new List<Tuple<QuestItem, int>>()
            {
                new Tuple<QuestItem, int>(questItems.Find(x => x.Name== "Spider Web"), 4)
            };
            List<Item> inTheWebRewards = new List<Item>()
            {
                items.Find(x => x.Name == "Worn Leather Helmet")
            };
            Quest inTheWeb = new Quest("In The Web",
                "@15|I can't see anything in my @2|basement",
                emptyEnemies, inTheWebQuestItems,
                20, inTheWebRewards);

            List<Tuple<Enemy, int>> filthyCockroachesEnemies = new List<Tuple<Enemy, int>>
            {
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Cockroach"), 7)
            };
            Quest filthyCockroaches = new Quest("Filthy Cockroaches",
                "@15|These stingy @9|insects @15|stole half of my @6|coins",
                filthyCockroachesEnemies, emptyQuestItems,
                40, emptyRewards);

            List<Tuple<QuestItem, int>> venomousFarmItems = new List<Tuple<QuestItem, int>>()
            {
                new Tuple<QuestItem, int>(questItems.Find(x => x.Name == "Snake Venom"), 5),
                new Tuple<QuestItem, int>(questItems.Find(x => x.Name == "Frog Venom"), 4)
            };
            List<Item> venomousFarmRewards = new List<Item>()
            {
                items.Find(x => x.Name == "Branch"),
                items.Find(x => x.Name == "Tobacco Leaf"),
                items.Find(x => x.Name == "Rice")
            };
            Quest venomousFarm = new Quest("Venomous Farm",
                "@15|Something causes our @9|people @15|to vomit extremely often",
                emptyEnemies, venomousFarmItems,
                0, venomousFarmRewards);

            List<Tuple<Enemy, int>> whiteWidowEnemies = new List<Tuple<Enemy, int>>
            {
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Black Widow"), 1)
            };
            Quest whiteWidow = new Quest("White Widow",
                "You have the best opportunity to stop this madness",
                whiteWidowEnemies, emptyQuestItems,
                50, emptyRewards);

            Quest myBrotherHenry = new Quest("My Brother Henry",
                "@15|Please talk to @9|Henry@15|, when he arrives. He knows more about that sitaution",
                emptyEnemies, emptyQuestItems,
                0, emptyRewards, true);

            List<Tuple<Enemy, int>> bloodyDenEnemies = new List<Tuple<Enemy, int>>()
            {
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Blood Spider"), 7)
            };
            List<Item> bloodyDenRewards = new List<Item>()
            {
                items.Find(x => x.Name == "Wooden Sword"),
            };
            Quest bloodyDen = new Quest("Bloody Den",
                "You gonna give them hell",
                bloodyDenEnemies, emptyQuestItems,
                50, bloodyDenRewards);

            List<Tuple<QuestItem, int>> spiderKingItems = new List<Tuple<QuestItem, int>>()
            {
                new Tuple<QuestItem, int>(questItems.Find(x => x.Name == "Tarantula Egg"), 4),
                new Tuple<QuestItem, int>(questItems.Find(x => x.Name == "Crab Spider Eye"), 4),
                new Tuple<QuestItem, int>(questItems.Find(x => x.Name == "Orange Blob"), 4),
            };
            List<Item> spiderKingRewards = new List<Item>()
            {
                items.Find(x => x.Name == "Quilted Armor"),
                items.Find(x => x.Name == "Bowl of Spider Soup")
            };
            Quest spiderKing = new Quest("Spider King",
                "@15|I'm going to prepare an amazing @14|soup",
                emptyEnemies, spiderKingItems,
                100, spiderKingRewards);

            List<Tuple<Enemy, int>> backInTimeEnemies = new List<Tuple<Enemy, int>>()
            {
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Ant"), 20),
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Bug"), 8),
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Spider"), 6),
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Cockroach"), 5)
            };
            List<Item> backInTimeRewards = new List<Item>()
            {
                items.Find(x => x.Name == "Worn Bronze Boots"),
                items.Find(x => x.Name == "Wooden Axe")
            };
            Quest backInTime = new Quest("Back In Time",
                "Show some respect to the past",
                backInTimeEnemies, emptyQuestItems,
                0, backInTimeRewards);

            List<Tuple<Enemy, int>> laboursOfHerculesEnemies = new List<Tuple<Enemy, int>>()
            {
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Mantis"), 12),
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Frog"), 7),
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Snake"), 3)
            };
            List<Item> laboursOfHerculesRewards = new List<Item>()
            {
                items.Find(x => x.Name == "Vegan Cheese Slice"),
                items.Find(x => x.Name == "Worn Bronze Helmet"),
            };
            Quest laboursOfHercules = new Quest("Labours of Hercules",
                "@15|You need to clean the @2|shed properly",
                laboursOfHerculesEnemies, emptyQuestItems,
                150, laboursOfHerculesRewards);

            List<Tuple<QuestItem, int>> farmFaunaItems = new List<Tuple<QuestItem, int>>()
            {
                new Tuple<QuestItem, int>(questItems.Find(x => x.Name == "Spider Leg"), 3),
                new Tuple<QuestItem, int>(questItems.Find(x => x.Name == "Mantis Antennae"), 4),
                new Tuple<QuestItem, int>(questItems.Find(x => x.Name == "Blood Spider Blood"), 5),
                new Tuple<QuestItem, int>(questItems.Find(x => x.Name == "Fox Tail"), 6),
                new Tuple<QuestItem, int>(questItems.Find(x => x.Name == "Wolf Paw"), 7),
            };
            List<Item> farmFaunaRewards = new List<Item>()
            {
                items.Find(x => x.Name == "Farmenstein"),
                items.Find(x => x.Name == "Farmenstein"),
                items.Find(x => x.Name == "Farmenstein"),
            };
            Quest farmFauna = new Quest("Farm Fauna",
                "@15|I hope you recognize all of @12|them",
                emptyEnemies, farmFaunaItems,
                1500, farmFaunaRewards);

            List<Tuple<Enemy, int>> witchHuntEnemies = new List<Tuple<Enemy, int>>
            {
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Witch"), 1)
            };
            Quest witchHunt = new Quest("Witch Hunt",
                "That's your way to go",
                witchHuntEnemies, emptyQuestItems,
                0, emptyRewards);
            #endregion

            #region Amfurce
            List<Item> veganRevolutionRewards = new List<Item>()
            {
                items.Find(x => x.Name == "Vegan Cheese Slice"),
                items.Find(x => x.Name == "Vegan Slice of Ham"),
                items.Find(x => x.Name == "Roll"),
                items.Find(x => x.Name == "Vegan Butter"),
                items.Find(x => x.Name == "Salt"),
                items.Find(x => x.Name == "Pepper"),
            };
            Quest veganRevolution = new Quest("Vegan Revolution",
                "@15|Ask @9|Sam @15|about the @10|place",
                emptyEnemies, emptyQuestItems, 0, veganRevolutionRewards, true);

            List<Tuple<Enemy, int>> intrusiveThoughtsEnemies = new List<Tuple<Enemy, int>>
            {
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Protein Deficiency"), 4),
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Plant Feelings"), 6),
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Bacon Tho"), 7),
            };
            List<Item> intrusiveThoughtsRewards = new List<Item>()
            {
                items.Find(x => x.Name == "Vegan Sandwich"),
            };
            Quest intrusiveThoughts = new Quest("Intrusive Thoughts",
                "First of all, you have to challenge and win against your former self",
                intrusiveThoughtsEnemies, emptyQuestItems, 0, emptyRewards);

            List<Tuple<Enemy, int>> pastYouEnemies = new List<Tuple<Enemy, int>>
            {
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Past You"), 1),
            };
            List<Item> pastYouRewards = new List<Item>()
            {
                items.Find(x => x.Name == "Artificial Leather Helmet"),
                items.Find(x => x.Name == "Artificial Leather Armor"),
                items.Find(x => x.Name == "Artificial Leather Legs"),
                items.Find(x => x.Name == "Artificial Leather Boots"),
                items.Find(x => x.Name == "BIO Wooden Stick"),
            };
            Quest pastYou = new Quest("Past You",
                "Now is the time to truly win against yourself",
                pastYouEnemies, emptyQuestItems, 0, pastYouRewards);

            List<Tuple<QuestItem, int>> exoticIngredientsItems = new List<Tuple<QuestItem, int>>()
            {
                new Tuple<QuestItem, int>(questItems.Find(x => x.Name == "Tempeh"), 12),
                new Tuple<QuestItem, int>(questItems.Find(x => x.Name == "Pak Choi"), 12),
                new Tuple<QuestItem, int>(questItems.Find(x => x.Name == "Soy Sauce"), 8),
                new Tuple<QuestItem, int>(questItems.Find(x => x.Name == "Sesame Oil"), 8)
            };
            List<Item> exoticIngredientsRewards = new List<Item>()
            {
                items.Find(x => x.Name == "Tempeh Stir Fry"),
                items.Find(x => x.Name == "Tempeh Stir Fry"),
                items.Find(x => x.Name == "Tempeh Stir Fry"),
                items.Find(x => x.Name == "Tempeh Stir Fry"),
                items.Find(x => x.Name == "Tempeh Stir Fry"),
                items.Find(x => x.Name == "Empathy Sword"),
            };
            Quest exoticIngredients = new Quest("Exotic Ingredients",
                "@15|I'm gonna prepare something @14|special @15|for you",
                emptyEnemies, exoticIngredientsItems, 5000, exoticIngredientsRewards);

            List<Tuple<QuestItem, int>> stolenRecipeItems = new List<Tuple<QuestItem, int>>()
            {
                new Tuple<QuestItem, int>(questItems.Find(x => x.Name == "BIO Leather"), 10),
                new Tuple<QuestItem, int>(questItems.Find(x => x.Name == "Fragment of Recipe"), 10),
            };
            List<Item> stolenRecipeRewards = new List<Item>()
            {
                items.Find(x => x.Name == "BIO Leather Ball"),
            };
            Quest stolenRecipe = new Quest("Stolen Recipe",
                "@9|They @15|think it's gonna stop us",
                emptyEnemies, stolenRecipeItems, 0, stolenRecipeRewards);

            List<Tuple<Enemy, int>> thisIsForbiddenEnemies = new List<Tuple<Enemy, int>>
            {
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Slaughterhouse Owner"), 1),
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Poacher"), 1),
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Hunter"), 1),
            };
            List<Item> thisIsForbiddenRewards = new List<Item>()
            {
                items.Find(x => x.Name == "BIO Staff of Flowing Water"),
            };
            Quest thisIsForbidden = new Quest("This Is Forbidden",
                "We don't go there",
                thisIsForbiddenEnemies, emptyQuestItems, 0, thisIsForbiddenRewards);

            List<Tuple<Enemy, int>> theEndEnemies = new List<Tuple<Enemy, int>>
            {
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Ruined King"), 1),
            };
            Quest theEnd = new Quest("The End",
                "Your final task",
                theEndEnemies, emptyQuestItems, 0, emptyRewards);
            #endregion

            quests = new List<Quest>()
            {
                #region Farm
                peterProblem,
                antVacuum,
                inTheWeb,

                filthyCockroaches,
                venomousFarm,
                whiteWidow,

                myBrotherHenry,
                bloodyDen,
                spiderKing,

                backInTime,
                laboursOfHercules,
                farmFauna,
                witchHunt,
                #endregion

                #region Amfurce
                veganRevolution,
                intrusiveThoughts,
                pastYou,

                exoticIngredients,

                stolenRecipe,
                thisIsForbidden,

                theEnd
                #endregion
            };
        }

        void GenerateAreas()
        {
            #region Farm
            Area basement = new Area("Basement", new List<Tuple<Enemy, int>>
            {
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Cockroach"), 10),
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Spider"), 4),
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Bug"), 3),
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Ant"), 1)
            });

            Area shed = new Area("Shed", new List<Tuple<Enemy, int>>
            {
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Snake"), 10),
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Frog"), 5),
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Mantis"), 3),
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Cockroach"), 1)
            });

            Area blackWidow = new Area("Black Widow", new List<Tuple<Enemy, int>>
            {
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Black Widow"), 1)
            });

            Area spiderDen = new Area("Spider Den", new List<Tuple<Enemy, int>>
            {
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Orange Spider"), 9),
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Crab Spider"), 7),
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Banana Spider"), 5),
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Tarantula"), 4),
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Blood Spider"), 1)
            });

            Area forest = new Area("Forest", new List<Tuple<Enemy, int>>
            {
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Wolf"), 7),
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Fox"), 6),
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Hedge Hog"), 6),
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Owl"), 5),
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Squirrel"), 1)
            });

            Area witchHut = new Area("Witch Hut", new List<Tuple<Enemy, int>>
            {
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Witch"), 1)
            });
            #endregion

            #region Amfurce
            Area mind = new Area("Mind", new List<Tuple<Enemy, int>>
            {
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Protein Deficiency"), 6),
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Plant Feelings"), 3),
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Bacon Tho"), 1)
            });

            Area shadowLand = new Area("Shadow Land", new List<Tuple<Enemy, int>>
            {
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Past You"), 1)
            });

            Area suburb = new Area("Suburb", new List<Tuple<Enemy, int>>
            {
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Obese Hamburger Eater"), 4),
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Shark Fin Lover"), 2),
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Vicious Meat Eater"), 1),
            });

            Area hospital = new Area("Hospital", new List<Tuple<Enemy, int>>
            {
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Cancer Patient"), 4),
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Madman"), 2),
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Patient with Hypertension"), 1),
            });

            Area complaintsRoom = new Area("Complaints Room", new List<Tuple<Enemy, int>>
            {
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Butcher"), 4),
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Fisherman"), 2),
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Bullfighter"), 1),
            });

            Area outsideTheCity = new Area("Outside the City", new List<Tuple<Enemy, int>>
            {
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Zoo Worker"), 7),
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Factory Farmer"), 5),
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Beekeeper"), 2),
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Cow Inseminator"), 1)
            });

            Area forbiddenArea = new Area("Forbidden Area", new List<Tuple<Enemy, int>>
            {
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Slaughterhouse Owner"), 4),
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Poacher"), 2),
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Hunter"), 1),
            });

            Area cityHall = new Area("City Hall", new List<Tuple<Enemy, int>>
            {
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Desperated Bacon Lover"), 4),
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Forious Egg Eater"), 2),
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Angry Meat Lover"), 1),
            });

            Area throneRoom = new Area("Throne Room", new List<Tuple<Enemy, int>>
            {
                new Tuple<Enemy, int>(enemies.Find(x => x.Name == "Ruined King"), 1),
            });
            #endregion

            areas = new List<Area>()
            {
                #region Farm
                basement,
                shed,
                blackWidow,

                spiderDen,
                forest,
                witchHut,
                #endregion

                #region Amfurce
                mind,
                shadowLand,

                suburb,
                hospital,
                complaintsRoom,

                outsideTheCity,
                forbiddenArea,

                cityHall,
                throneRoom
                #endregion
            };
        }

        void GenerateNpcs()
        {
            #region Farm
            string peterHello = "Be quick, I'm pretty busy";
            string peterWork = "@15|I @6|buy @15|many different things\n";
            string peterPlace = "@15|I love this @10|place@15|, but my @2|basement @15|got ruined, because of these @9|insects";
            List<Tuple<Quest, Quest>> peterQuests = new List<Tuple<Quest, Quest>>()
            {
                new Tuple<Quest, Quest>(quests.Find(x => x.Name == "Ant Vacuum"), quests.Find(x => x.Name == "Peter Problem")),
                new Tuple<Quest, Quest>(quests.Find(x => x.Name == "In The Web"), quests.Find(x => x.Name == "Ant Vacuum"))
            };
            Merchant peter = new Merchant("Peter", peterHello, peterWork, peterPlace, 
                peterQuests, 1.0,
                null, quests.Find(x => x.Name == "Peter Problem"));


            string adamHello = "@15|Hello, glad to see new face on our @10|farm";
            string adamWork = "@15|I have little @6|store @15|over there, take a look inside\n";
            string adamPlace = "@15|Life here was great up until last year\n" +
                "Suddenly all these things started to come from the @2|forest";
            List<Tuple<Quest, Quest>> adamQuests = new List<Tuple<Quest, Quest>>()
            {
                new Tuple<Quest, Quest>(quests.Find(x => x.Name == "Filthy Cockroaches"), quests.Find(x => x.Name == "In The Web")),
                new Tuple<Quest, Quest>(quests.Find(x => x.Name == "Venomous Farm"), quests.Find(x => x.Name == "In The Web"))
            };
            List<Item> adamItems = new List<Item>()
            {
                items.Find(x => x.Name == "Leather Helmet"),
                items.Find(x => x.Name == "Worn Leather Armor"),
                items.Find(x => x.Name == "Worn Leather Legs"),
                items.Find(x => x.Name == "Worn Leather Boots"),
                items.Find(x => x.Name == "Leather Boots"),
                items.Find(x => x.Name == "Wooden Stick"),
                items.Find(x => x.Name == "Rock")
            };
            Shopkeeper adam = new Shopkeeper("Adam", adamHello, adamWork, adamPlace,
                adamQuests, 3.0, adamItems);


            string marcusHello = "Glad to see you";
            string marcusWork = "@15|I @6|work @15|on field, I can sell you some @14|crops\n";
            string marcusPlace = "@15|Someone needs to finally get rid of these @9|insects";
            List<Tuple<Quest, Quest>> marcusQuests = new List<Tuple<Quest, Quest>>()
            {
                new Tuple<Quest, Quest>(quests.Find(x => x.Name == "White Widow"), quests.Find(x => x.Name == "Venomous Farm")),
                new Tuple<Quest, Quest>(quests.Find(x => x.Name == "My Brother Henry"), quests.Find(x => x.Name == "White Widow")),
                new Tuple<Quest, Quest>(quests.Find(x => x.Name == "Bloody Den"), quests.Find(x => x.Name == "White Widow")),
                new Tuple<Quest, Quest>(quests.Find(x => x.Name == "Spider King"), quests.Find(x => x.Name == "White Widow"))
            };
            List<Item> marcusItems = new List<Item>()
            {
                items.Find(x => x.Name == "Millet"),             
                items.Find(x => x.Name == "Tobacco Leaf"),
                items.Find(x => x.Name == "Apple Juice"),
                items.Find(x => x.Name == "Kidney Beans"),
                items.Find(x => x.Name == "Aubergine"),
                items.Find(x => x.Name == "Courgette"),          
                items.Find(x => x.Name == "Pear")
            };
            Shopkeeper marcus = new Shopkeeper("Marcus", marcusHello, marcusWork, marcusPlace,
                marcusQuests, 4.0, marcusItems,
                quests.Find(x => x.Name == "Venomous Farm"), null);


            string henryHello = "@15|Hello, my brother @9|Marcus @15|told me about you";
            string henryWork = "@15|I sell beatufily crafted @8|bronze set @15|from neighbour @10|city\n";
            string henryPlace = "@9|Marcus @15|said you are ready and willing to face danger coming from the @2|forest\n" +
                "@15|I can prepare you for that fight, but beware there were many before you";
            List<Tuple<Quest, Quest>> henryQuests = new List<Tuple<Quest, Quest>>()
            {
                new Tuple<Quest, Quest>(quests.Find(x => x.Name == "Back In Time"), 
                    quests.Find(x => x.Name == "My Brother Henry")),
                new Tuple<Quest, Quest>(quests.Find(x => x.Name == "Labours of Hercules"), 
                    quests.Find(x => x.Name == "My Brother Henry")),
                new Tuple<Quest, Quest>(quests.Find(x => x.Name == "Farm Fauna"),
                    quests.Find(x => x.Name == "Labours of Hercules")),
                new Tuple<Quest, Quest>(quests.Find(x => x.Name == "Witch Hunt"),
                    quests.Find(x => x.Name == "Farm Fauna")),
            };
            List<Item> henryItems = new List<Item>()
            {             
                items.Find(x => x.Name == "Bronze Helmet"),
                items.Find(x => x.Name == "Bronze Armor"),
                items.Find(x => x.Name == "Bronze Legs"),
                items.Find(x => x.Name == "Bronze Boots"),
                items.Find(x => x.Name == "Bronze Sword")
            };
            Shopkeeper henry = new Shopkeeper("Henry", henryHello, henryWork, henryPlace,
                henryQuests, 8.0, henryItems,
                quests.Find(x => x.Name == "Bloody Den"), quests.Find(x => x.Name == "My Brother Henry"));
            #endregion

            #region Amfurce
            string samHello = "@15|Welcome to our little big revolution";
            string samWork = "@15|Come on you need some @14|food";
            string samPlace = "@15|If you want to be respected in @10|Amfurce @15|\nyou have to show respect towards @9|animals";
            List<Tuple<Quest, Quest>> samQuests = new List<Tuple<Quest, Quest>>()
            {
                new Tuple<Quest, Quest>(quests.Find(x => x.Name == "Intrusive Thoughts"), 
                    quests.Find(x => x.Name == "Vegan Revolution")),
                new Tuple<Quest, Quest>(quests.Find(x => x.Name == "Past You"),
                    quests.Find(x => x.Name == "Intrusive Thoughts")),
                new Tuple<Quest, Quest>(quests.Find(x => x.Name == "Exotic Ingredients"),
                    quests.Find(x => x.Name == "Past You")),
                new Tuple<Quest, Quest>(quests.Find(x => x.Name == "The End"),
                    quests.Find(x => x.Name == "Stolen Recipe"))
            };
            List<Item> samItems = new List<Item>()
            {
                items.Find(x => x.Name == "Vegan Cheese Slice"),
                items.Find(x => x.Name == "Vegan Slice of Ham"),
                items.Find(x => x.Name == "Bread"),
                items.Find(x => x.Name == "Salt"),
                items.Find(x => x.Name == "Orange"),
                items.Find(x => x.Name == "Strawberry"),
                items.Find(x => x.Name == "Smoked Paprika"),
                items.Find(x => x.Name == "Soy Milk"),
                items.Find(x => x.Name == "Seitan")
            };
            Shopkeeper sam = new Shopkeeper("Sam", samHello, samWork, samPlace,
                samQuests, 2.0, samItems,
                null, quests.Find(x => x.Name == "Vegan Revolution"));

            string ericHello = "You showed us your value";
            string ericWork = "@15|Someone needs to keep all of this stuff";
            string ericPlace = "@15|Beatiful @10|city@15|, less beatiful @9|people";
            List<Tuple<Quest, Quest>> ericQuests = new List<Tuple<Quest, Quest>>()
            {
                new Tuple<Quest, Quest>(quests.Find(x => x.Name == "Stolen Recipe"),
                    quests.Find(x => x.Name == "Exotic Ingredients")),
            };
            Merchant eric = new Merchant("Eric", ericHello, ericWork, ericPlace,
                ericQuests, 1.0,
                quests.Find(x => x.Name == "Past You"), null);

            string danHello = "@15|Show some empathy to all living @9|things";
            string danWork = "I want to show you something beatiful";
            string danPlace = "@15|It's better than ever";
            List<Tuple<Quest, Quest>> danQuests = new List<Tuple<Quest, Quest>>()
            {
                new Tuple<Quest, Quest>(quests.Find(x => x.Name == "This Is Forbidden"),
                    quests.Find(x => x.Name == "Exotic Ingredients")),
            };
            List<Item> danItems = new List<Item>()
            {
                items.Find(x => x.Name == "Empathy Helmet"),
                items.Find(x => x.Name == "Empathy Armor"),
                items.Find(x => x.Name == "Empathy Legs"),
                items.Find(x => x.Name == "Empathy Boots"),
                items.Find(x => x.Name == "BIO Tofu")
            };
            Shopkeeper dan = new Shopkeeper("Dan", danHello, danWork, danPlace,
                danQuests, 7.0, danItems,
                quests.Find(x => x.Name == "Exotic Ingredients"), null);
            #endregion

            npcs = new List<NPC>()
            {
                peter,
                adam,
                marcus,
                henry,

                sam,
                eric,
                dan
            };
        }

        void GenerateCities()
        {
            cities = new List<City>();

            #region Amfurce
            List<NPC> amfurceNpcs = new List<NPC>()
            {
                npcs.Find(x => x.Name == "Sam"),
                npcs.Find(x => x.Name == "Eric"),
                npcs.Find(x => x.Name == "Dan")
            };
            List<Tuple<Area, Quest>> amfurceAreas = new List<Tuple<Area, Quest>>()
            {
                new Tuple<Area, Quest>(areas.Find(x => x.Name == "Outside the City"), quests.Find(x => x.Name == "Exotic Ingredients")),
                new Tuple<Area, Quest>(areas.Find(x => x.Name == "City Hall"), quests.Find(x => x.Name == "This Is Forbidden"))
            };
            List<Tuple<Area, Quest>> amfurceTemporaryAreas = new List<Tuple<Area, Quest>>()
            {
                new Tuple<Area, Quest>(areas.Find(x => x.Name == "Mind"), quests.Find(x => x.Name == "Intrusive Thoughts")),
                new Tuple<Area, Quest>(areas.Find(x => x.Name == "Shadow Land"), quests.Find(x => x.Name == "Past You")),
                new Tuple<Area, Quest>(areas.Find(x => x.Name == "Suburb"), quests.Find(x => x.Name == "Exotic Ingredients")),
                new Tuple<Area, Quest>(areas.Find(x => x.Name == "Hospital"), quests.Find(x => x.Name == "Exotic Ingredients")),
                new Tuple<Area, Quest>(areas.Find(x => x.Name == "Complaints Room"), quests.Find(x => x.Name == "Exotic Ingredients")),
                new Tuple<Area, Quest>(areas.Find(x => x.Name == "Forbidden Area"), quests.Find(x => x.Name == "This Is Forbidden")),
                new Tuple<Area, Quest>(areas.Find(x => x.Name == "Throne Room"), quests.Find(x => x.Name == "The End"))
            };

            City amfurce = amfurce = new City("Amfurce", quests.Find(x => x.Name == "Vegan Revolution"),
                null, null,
                amfurceNpcs, amfurceAreas, amfurceTemporaryAreas);
            #endregion

            #region Farm
            List<NPC> farmNpcs = new List<NPC>()
            {
                npcs.Find(x => x.Name == "Peter"),
                npcs.Find(x => x.Name == "Adam"),
                npcs.Find(x => x.Name == "Marcus"),
                npcs.Find(x => x.Name == "Henry"),        
            };
            List<Tuple<Area, Quest>> farmAreas = new List<Tuple<Area, Quest>>()
            {
                new Tuple<Area, Quest>(areas.Find(x => x.Name == "Basement"), quests.Find(x => x.Name == "Peter Problem")),
                new Tuple<Area, Quest>(areas.Find(x => x.Name == "Shed"), quests.Find(x => x.Name == "In The Web")),
                new Tuple<Area, Quest>(areas.Find(x => x.Name == "Spider Den"), quests.Find(x => x.Name == "White Widow")),
                new Tuple<Area, Quest>(areas.Find(x => x.Name == "Forest"), quests.Find(x => x.Name == "My Brother Henry")),
            };
            List<Tuple<Area, Quest>> temporaryFarmAreas = new List<Tuple<Area, Quest>>()
            {
                new Tuple<Area, Quest>(areas.Find(x => x.Name == "Black Widow"), quests.Find(x => x.Name == "White Widow")),
                new Tuple<Area, Quest>(areas.Find(x => x.Name == "Witch Hut"), quests.Find(x => x.Name == "Witch Hunt"))
            };

            City farm = new City("Farm", quests.Find(x => x.Name == "Peter Problem"), 
                quests.Find(x => x.Name == "Witch Hunt"), amfurce,
                farmNpcs, farmAreas, temporaryFarmAreas);
            #endregion

            cities.Add(farm);
            cities.Add(amfurce);
        }
    }
}
