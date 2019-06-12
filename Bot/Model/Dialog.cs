using System;

namespace Bot
{
    public class DialogQuestion
    {
        public string Name { get; set; }
        public string Message { get; set; }
        public string ForPlayer { get; set; }
        public Func<Inventory, string> DynamicMessage { get; set; }
        public string Photo { get; set; }
        public DialogAnswer[] Answers { get; set; } = { };
        public bool DisplayMap { get; set; }
        public char? MapIcon { get; set; }
        public bool PreventMove { get; set; }
    }

    public class DialogAnswer
    {
        public string Message { get; set; }
        public bool IsHidden { get; set; }
        public Func<Inventory, bool> Available { get; set; } = (i => true);

        public string MoveToDialog { get; set; }
        public Func<int, string, int> MoveToPos { get; set; }

        public Action<Inventory> ChangeInventory { get; set; }
        public Func<int, string, string> ChangeMap { get; set; }
    }
}
