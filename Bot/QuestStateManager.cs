using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Bot
{
    public class BotState
    {
        public QuestState QuestState { get; set; }
        public int? PreviousMessageHash { get; set; }
        public int? PreviousMessageId { get; set; }
        public bool PreviousMessageIsText { get; set; }
    }

    public class QuestStateManager
    {
        private readonly Dictionary<string, BotState> questStateDict;
        private const string FolderPath = "BotStates";

        public BotState GetState(string chatId)
        {
            var path = BuildFilePath(chatId);
            return questStateDict.ContainsKey(chatId)
                ? questStateDict[chatId]
                : (questStateDict[chatId] = File.Exists(path)
                    ? JsonConvert.DeserializeObject<BotState>(File.ReadAllText(path))
                    : null);
        }

        public void SetState(string chatId, BotState state)
        {
            var botState = questStateDict[chatId] = state;
            var serialized = JsonConvert.SerializeObject(botState, Formatting.Indented);

            if (!Directory.Exists(FolderPath)) {
                Directory.CreateDirectory(FolderPath);
            }
            File.WriteAllText(BuildFilePath(chatId), serialized);
        }

        private static string BuildFilePath(string chatId)
        {
            return Path.Combine(FolderPath, chatId + ".json");
        }

        public QuestStateManager()
        {
            questStateDict = new Dictionary<string, BotState>();
        }
    }
}
