using System.Collections.Generic;

namespace Bot
{
    public class Inventory
    {
        public HashSet<string> Items { get; set; } = new HashSet<string>();
        public int Money { get; set; }

        public Inventory Give(string itemName)
        {
            Items.Add(itemName);
            return this;
        }

        public Inventory Take(string itemName)
        {
            Items.Remove(itemName);
            return this;
        }

        public bool Has(string itemName) => Items.Contains(itemName);
    }

    public class Journal
    {
        public HashSet<string> AllQuests { get; set; } = new HashSet<string>();
        public HashSet<string> FinishedQuests { get; set; } = new HashSet<string>();

        public Journal Open(string questName)
        {
            AllQuests.Add(questName);
            return this;
        }

        public Journal Finish(string questName)
        {
            AllQuests.Add(questName);
            FinishedQuests.Add(questName);
            return this;
        }

        public bool IsKnown(string itemName) => AllQuests.Contains(itemName);
        public bool IsOpen(string itemName) => AllQuests.Contains(itemName) && !FinishedQuests.Contains(itemName);
        public bool IsFinished(string itemName) => FinishedQuests.Contains(itemName);
    }
}