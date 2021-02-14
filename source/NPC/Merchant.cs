using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeganRPG
{
    class Merchant : NPC
    {
        double valueMultiplier;

        public Merchant(string name, string helloMessage, string workMessage, string placeMessage, 
            List<Tuple<Quest, Quest>> quests, double valueMultipler, 
            Quest questToActivate = null, Quest placeQuest = null) : 
            base(name, helloMessage, workMessage, placeMessage, quests, questToActivate, placeQuest)
        {
            ValueMultiplier = valueMultipler;
        }

        public override void Work(Player player)
        {
            while (true)
            {
                Console.Clear();

                Util.WriteColorString(WorkMessage, true);

                Util.Write("1. Sell ");
                Util.WriteLine("items", ConsoleColor.DarkGray);

                Util.Write("2. Sell ");
                Util.WriteLine("consumables", ConsoleColor.Yellow);

                Util.WriteLine("\n0. Exit");

                var decision = Console.ReadKey();

                if (decision.Key == ConsoleKey.NumPad0)
                {
                    break;
                }
                else if (decision.Key == ConsoleKey.NumPad1)
                {
                    SellItems(player);
                }
                else if (decision.Key == ConsoleKey.NumPad2)
                {
                    SellConsumables(player);
                }
            }
        }

        private void SellItems(Player player)
        {
            if (player.ItemStash.Count == 0)
            {
                Console.Clear();

                Util.WriteLine("You don't have anything to sell.");

                Console.ReadKey();

                return;
            }

            int site = 0;
            int maxSite;

            while (true)
            {
                maxSite = (player.ItemStash.Count - 1) / 7;
                if (site > maxSite)
                {
                    site = maxSite;
                }

                Console.Clear();

                player.OrderItemStashList();

                Util.Write("Gold", ConsoleColor.DarkYellow);
                Util.Write(": ");
                Util.WriteLine(player.Gold + "\n", ConsoleColor.DarkYellow);

                Util.Write("Item ", ConsoleColor.DarkGray);
                Util.WriteLine("stash: " + (site + 1) + " / " + (maxSite + 1));
                for (int i = 0; i < 7; ++i)
                {
                    if (player.ItemStash.Count <= i + (site * 7))
                    {
                        break;
                    }

                    Util.Write(i + 1 + " ");
                    player.ItemStash[i + (site * 7)].Info(false);
                    Util.Write(" - ");
                    Util.Write(Convert.ToInt32((player.ItemStash[i + (site * 7)].Value() * valueMultiplier))
                        + "G\n", ConsoleColor.DarkYellow);
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
                    if (decision + (site * 7) < player.ItemStash.Count + 1)
                    {
                        player.Gold += Convert.ToInt32(player.ItemStash[decision - 1 + (site * 7)].Value() * valueMultiplier);

                        Util.Write("\nYou sold ");
                        Util.Write(player.ItemStash[decision - 1 + (site * 7)].Name, ConsoleColor.DarkGray);
                        Util.Write(" for ");
                        Util.WriteLine(Convert.ToInt32(player.ItemStash[decision - 1 + (site * 7)].Value() * valueMultiplier)
                            + " G", ConsoleColor.DarkYellow);

                        Console.ReadKey();

                        player.ItemStash.RemoveAt(decision - 1 + (site * 7));
                    }
                }
            }
        }

        private void SellConsumables(Player player)
        {
            if (player.Consumables.Count == 0)
            {
                Console.Clear();

                Util.WriteLine("You don't have anything to sell.");

                Console.ReadKey();

                return;
            }

            int site = 0;
            int maxSite;

            while (true)
            {
                maxSite = (player.Consumables.Count - 1) / 7;
                if (site > maxSite)
                {
                    site = maxSite;
                }

                Console.Clear();

                player.OrderConsumablesList();

                Util.Write("Gold", ConsoleColor.DarkYellow);
                Util.Write(": ");
                Util.WriteLine(player.Gold + "\n", ConsoleColor.DarkYellow);

                Util.Write("Consumables", ConsoleColor.Yellow);
                Util.WriteLine(": " + (site + 1) + " / " + (maxSite + 1));
                for (int i = 0; i < 7; ++i)
                {
                    if (player.Consumables.Count <= i + (site * 7))
                    {
                        break;
                    }

                    Util.Write(i + 1 + " ");
                    player.Consumables[i + (site * 7)].Info(false);
                    Util.Write(" - " + player.Consumables[i].Count + ", ");
                    Util.Write(Convert.ToInt32(player.Consumables[i + (site * 7)].Value() * valueMultiplier)
                        + "G\n", ConsoleColor.DarkYellow);
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
                    if (decision + (site * 7) < player.Consumables.Count + 1)
                    {
                        player.Gold += Convert.ToInt32(player.Consumables[decision - 1 + (site * 7)].Value() * valueMultiplier);

                        Util.Write("\nYou sold ");
                        Util.Write(player.Consumables[decision - 1 + (site * 7)].Name);
                        Util.Write(" for ");
                        Util.WriteLine(Convert.ToInt32(player.Consumables[decision - 1 + (site * 7)].Value() * valueMultiplier)
                            + " G", ConsoleColor.DarkYellow);

                        Console.ReadKey();

                        if (player.Consumables[decision - 1 + (site * 7)].Count == 1)
                        {
                            player.Consumables.RemoveAt(decision - 1 + (site * 7));
                        }
                        else
                        {
                            player.Consumables[decision - 1 + (site * 7)].Count -= 1;
                        }
                    }
                }
            }
        }

        public double ValueMultiplier { get => valueMultiplier; set => valueMultiplier = value; }
    }
}
