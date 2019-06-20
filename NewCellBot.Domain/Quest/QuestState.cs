using NewCellBot.Domain.Quest.Model;

namespace NewCellBot.Domain.Quest
{
    public class QuestState
    {
        public int Pos { get; set; }
        public string Map { get; set; }
        public Inventory Inventory { get; set; }
        public Journal Journal { get; set; }
        public string OpenDialogName { get; set; }
    }
}