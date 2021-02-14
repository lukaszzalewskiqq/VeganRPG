using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeganRPG
{
    class City
    {
        string name;

        List<NPC> npcs;

        Quest startingQuest;
        Quest endQuest;
        City nextCity;

        // Tuple with Area and Quest that enables it
        List<Tuple<Area, Quest>> areas;

        // Tuple with Area and Quest during which Area is enabled
        List<Tuple<Area, Quest>> temporaryAreas;

        public City(string name, Quest startingQuest, Quest endQuest, City nextCity, 
            List<NPC> npcs, List<Tuple<Area, Quest>> areas,
            List<Tuple<Area, Quest>> temporaryAreas)
        {
            Name = name;

            StartingQuest = startingQuest;
            EndQuest = endQuest;
            NextCity = nextCity;

            Npcs = npcs;
            Areas = areas;
            TemporaryAreas = temporaryAreas;
        }

        public void People(Player player, List<Quest> activeQuests, List<Tuple<Enemy, int>> enemyTracker)
        {
            if (player.QuestsDone.Find(x => x == StartingQuest) == null && activeQuests.Find(x => x == startingQuest) == null)
            {
                Console.Clear();

                StartingQuest.Start(activeQuests, enemyTracker);

                Console.ReadKey();
            }

            while (true)
            {
                Console.Clear();

                Util.WriteLine("You see: ");

                for (int i = 0; i < Npcs.Count; ++i)
                {
                    if (Npcs[i].QuestToActivate == null || player.QuestsDone.Find(x => x == Npcs[i].QuestToActivate) != null)
                    {
                        Util.Write((i + 1) + " ");
                        Util.WriteLine(Npcs[i].Name, ConsoleColor.Blue);
                    }
                }

                Util.WriteLine("\n0. Exit");

                var decision = Util.NumpadKeyToInt(Console.ReadKey());

                if (decision == 0)
                {
                    break;
                }
                if (decision > 0 && decision - 1 < Npcs.Count)
                {
                    Npcs[decision - 1].Talk(player, activeQuests, enemyTracker);
                }
            }
        }

#pragma warning disable CS8632
        public Enemy? Outside(Player player, List<Quest> activeQuests)
#pragma warning restore CS8632
        {
            Enemy enemy = null;

            while (true)
            {
                Console.Clear();

                List<Tuple<Area, Quest>> possibleAreas = areas.Where(x => player.QuestsDone.Find(y => y == x.Item2) != null).ToList();
                
                foreach (var area in temporaryAreas)
                {
                    if (activeQuests.Contains(area.Item2))
                    {
                        possibleAreas.Add(area);
                    }
                }

                if (possibleAreas.Count > 0)
                {
                    Util.WriteLine("Go to: ");

                    for (int i = 0; i < possibleAreas.Count; ++i)
                    {
                        Util.Write((i + 1) + ". ");
                        Util.WriteLine(possibleAreas[i].Item1.Name, ConsoleColor.DarkGreen);
                    }

                    Util.WriteLine("\n0. Exit");

                    int decision = Util.NumpadKeyToInt(Console.ReadKey());

                    Console.Clear();

                    if (decision == 0)
                    {
                        break;
                    }
                    else if (decision > 0 && decision - 1 < possibleAreas.Count)
                    {
                        enemy = possibleAreas[decision - 1].Item1.LookForEnemy();

                        Util.Write("You met ");
                        Util.WriteLine(enemy.Name, ConsoleColor.Blue);

                        Console.ReadKey();

                        break;
                    }
                }
                else
                {
                    Util.WriteLine("You can't go anywhere");
                    Console.ReadKey();

                    break;
                }
            }

            return enemy;
        }

        internal List<NPC> Npcs { get => npcs; set => npcs = value; }
        public string Name { get => name; set => name = value; }
        internal List<Tuple<Area, Quest>> Areas { get => areas; set => areas = value; }
        internal List<Tuple<Area, Quest>> TemporaryAreas { get => temporaryAreas; set => temporaryAreas = value; }
        internal Quest StartingQuest { get => startingQuest; set => startingQuest = value; }
        internal Quest EndQuest { get => endQuest; set => endQuest = value; }
        internal City NextCity { get => nextCity; set => nextCity = value; }
    }
}
