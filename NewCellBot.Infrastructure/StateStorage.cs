using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using NewCellBot.Domain;

namespace NewCellBot.Infrastructure
{
    public class StateStorage
    {
        private readonly CloudTableClient _tableClient;

        public StateStorage(string connectionString)
        {
            CloudStorageAccount storageAccount;
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            }
            else
            {
                storageAccount = CloudStorageAccount.Parse(connectionString);
            }

            _tableClient = storageAccount.CreateCloudTableClient();
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
