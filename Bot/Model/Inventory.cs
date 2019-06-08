using System.Collections.Generic;

namespace Bot
{
    public class Inventory
    {
        public HashSet<string> Items { get; set; } = new HashSet<string>();
        public int Money { get; set; }
        public void Give(string itemName) => Items.Add(itemName);
        public bool Has(string itemName) => Items.Contains(itemName);
    }
}