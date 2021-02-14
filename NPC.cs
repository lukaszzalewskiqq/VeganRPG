using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeganRPG
{
    abstract class NPC
    {
        string name;

        string helloMessage;
        string workMessage;
        string placeMessage;

        List<Tuple<Quest, Quest>> quests;

        // Quest that has to be finished, before NPC shows up
        Quest questToActivate;

        // Quest that finishes after talking with NPC about place
        Quest placeQuest;

        public NPC(string name, string helloMessage, string workMessage, string placeMessage, List<Tuple<Quest, Quest>> quests,
            Quest questToActivate = null, Quest placeQuest = null)
        {
            Name = name;

            HelloMessage = helloMessage;
            WorkMessage = workMessage;
            PlaceMessage = placeMessage;

            Quests = quests;

            QuestToActivate = questToActivate;
            PlaceQuest = placeQuest;
        }

        public void Talk(Player player, List<Quest> activeQuests, List<Tuple<Enemy, int>> enemyTracker)
        {
            while (true)
            {
                Console.Clear();

                Util.WriteColorString("@15|Hello, I'm @9|" + Name + "\n");

                Util.WriteLine("\nSay: ");
                Util.WriteLine("1. Hello");

                Util.Write("2. Ask about his ");
                Util.WriteLine("work", ConsoleColor.DarkYellow);

                Util.Write("3. Ask about the ");
                Util.WriteLine("place", ConsoleColor.Green);

                if (quests.Count > 0)
                {
                    Util.Write("4. Ask if he has any ");
                    Util.Write("quest ", ConsoleColor.Red);
                    Util.WriteLine("for you");

                    Util.Write("5. Tell him about ");
                    Util.Write("quest ", ConsoleColor.Red);
                    Util.WriteLine("that you finished");
                }

                Util.WriteLine("\n0. Exit");

                var decision = Console.ReadKey();

                if (decision.Key == ConsoleKey.NumPad0)
                {
                    break;
                }
                else if (decision.Key == ConsoleKey.NumPad1)
                {
                    Hello();
                }
                else if (decision.Key == ConsoleKey.NumPad2)
                {
                    Work(player);
                }
                else if (decision.Key == ConsoleKey.NumPad3)
                {
                    Place(player, activeQuests, enemyTracker);
                }
                else if (decision.Key == ConsoleKey.NumPad4 && quests.Count > 0)
                {
                    GiveQuests(player, activeQuests, enemyTracker);
                }
                else if (decision.Key == ConsoleKey.NumPad5 && quests.Count > 0)
                {
                    FinishQuests(player, activeQuests, enemyTracker);
                }
            }
        }

        public void Hello()
        {
            Console.Clear();

            Util.WriteColorString(helloMessage);

            Console.ReadKey();
        }

        public abstract void Work(Player player);

        public void Place(Player player, List<Quest> activeQuests, List<Tuple<Enemy, int>> enemyTracker)
        {
            Console.Clear();

            if (placeQuest != null && activeQuests.Find(x => x == placeQuest) != null)
            {
                placeQuest.Finish(player, enemyTracker);
                activeQuests.Remove(placeQuest);

                Console.Clear();
            }

            Util.WriteColorString(placeMessage);

            Console.ReadKey();
        }

        private void GiveQuests(Player player, List<Quest> activeQuests, List<Tuple<Enemy, int>> enemyTracker)
        {
            Console.Clear();

            bool anyQuestGiven = false;

            foreach (var quest in quests)
            {
                if (player.QuestsDone.Find(x => x == quest.Item1) == null && activeQuests.Find(x => x == quest.Item1) == null)
                {
                    if (quest.Item2 == null || player.QuestsDone.Find(x => x == quest.Item2) != null)
                    {
                        quest.Item1.Start(activeQuests, enemyTracker);
                        quest.Item1.Info(player, enemyTracker);

                        Util.Write("\n");

                        anyQuestGiven = true;
                    }
                }
            }

            if (!anyQuestGiven)
            {
                Util.WriteColorString("@15|I don't have any @12|quest @15|for you\n");
            }

            Console.ReadKey();
        }

        private void FinishQuests(Player player, List<Quest> activeQuests, List<Tuple<Enemy, int>> enemyTracker)
        {
            Console.Clear();
            List<Quest> completedQuests = new List<Quest>();

            foreach (var quest in quests)
            {
                if (!quest.Item1.OutsideFinish)
                {
                    if (activeQuests.Find(x => x == quest.Item1) != null)
                    {
                        if (quest.Item1.CheckCompletion(player, enemyTracker))
                        {
                            quest.Item1.Finish(player, enemyTracker);
                            activeQuests.Remove(quest.Item1);

                            completedQuests.Add(quest.Item1);

                            Console.Clear();
                        }
                        else
                        {
                            quest.Item1.Info(player, enemyTracker);

                            Util.Write("\n");
                        }
                    }
                }
            }

            if (completedQuests.Count == 0)
            {
                Util.WriteColorString("@15|You didn't complete any @12|quest\n");

                Console.ReadKey();
            }
            else
            {
                foreach (var quest in completedQuests)
                {
                    quests.RemoveAll(x => x.Item1 == quest);
                }
            }      
        }

        public string Name { get => name; set => name = value; }
        internal List<Tuple<Quest, Quest>> Quests { get => quests; set => quests = value; }
        public string HelloMessage { get => helloMessage; set => helloMessage = value; }
        public string WorkMessage { get => workMessage; set => workMessage = value; }
        public string PlaceMessage { get => placeMessage; set => placeMessage = value; }
        internal Quest PlaceQuest { get => placeQuest; set => placeQuest = value; }
        internal Quest QuestToActivate { get => questToActivate; set => questToActivate = value; }
    }
}
