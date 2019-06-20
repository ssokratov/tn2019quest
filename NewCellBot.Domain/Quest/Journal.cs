using System.Collections.Generic;

namespace NewCellBot.Domain.Quest
{
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