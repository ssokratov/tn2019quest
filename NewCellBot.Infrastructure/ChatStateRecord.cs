using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Cosmos.Table;
using NewCellBot.Domain;
using Newtonsoft.Json;

namespace NewCellBot.Infrastructure
{
    public class BotStateRecord : TableEntity
    {
        public static BotStateRecord FromBotState(BotState state)
        {
            return new BotStateRecord
            {
                PartitionKey = state.ChatId.ToString(),
                RowKey = state.ChatId.ToString(),

                Payload = JsonConvert.SerializeObject(state)
            };
        }

        public BotState ToBotState()
        {
            return JsonConvert.DeserializeObject<BotState>(Payload);
        }

        public string Payload { get; set; }
    }
}
