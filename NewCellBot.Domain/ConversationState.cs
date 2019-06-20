using NewCellBot.Domain.Quest;

namespace NewCellBot.Domain
{
    public class BotState
    {
        public long ChatId { get; set; }

        public QuestState QuestState { get; set; }
        public ChatState ChatState { get; set; }
    }

    public class ChatState
    {
        public int? PreviousMessageHash { get; set; }
        public int? PreviousMessageId { get; set; }
        public bool PreviousMessageIsText { get; set; }
    }
}