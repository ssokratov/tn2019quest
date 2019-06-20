using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Documents.SystemFunctions;
using Microsoft.Extensions.Options;
using NewCellBot.Domain;

namespace NewCellBot.Infrastructure
{
    public class StateStorage
    {
        private CloudStorageAccount _storageAccount;
        private CloudTableClient _tableClient;

        public StateStorage(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                _storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            }

            _storageAccount = CloudStorageAccount.Parse(connectionString);
            _tableClient = _storageAccount.CreateCloudTableClient();
        }

        public async Task<BotState> GetStateAsync(long chatId)
        {
            var table = _tableClient.GetTableReference("ChatStates");

            await table.CreateIfNotExistsAsync();

            var result = await table.ExecuteAsync(TableOperation.Retrieve<BotStateRecord>(
                chatId.ToString(),
                chatId.ToString()));

            return ((BotStateRecord) result.Result)?.ToBotState();
        }

        public async Task StoreStateAsync(BotState state)
        {
            var table = _tableClient.GetTableReference("ChatStates");

            await table.CreateIfNotExistsAsync();

            await table.ExecuteAsync(TableOperation.InsertOrReplace(BotStateRecord.FromBotState(state)));
        }
    }
}
