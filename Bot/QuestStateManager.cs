using System.Collections.Generic;

namespace Bot
{
    public class QuestState
    {
        public QuestService Service { get; set; }
        public int? PreviousMessageHash { get; set; }
        public int? PreviousMessageId { get; set; }
        public bool PreviousMessageIsText { get; set; }
    }

    public class QuestStateManager
    {
        private readonly Dictionary<string, QuestState> questStateDict = new Dictionary<string, QuestState>();

        public QuestState GetState(string chatId) => questStateDict.ContainsKey(chatId)
            ? questStateDict[chatId]
            : null;

        public QuestState SetState(string chatId, QuestState state) => questStateDict[chatId] = state;
    }
}
