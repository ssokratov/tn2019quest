﻿using System.Collections.Generic;

namespace NewCellBot.Domain.Quest.Model
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
}