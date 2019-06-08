using System;
using System.Net;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace Bot
{
    class Program
    {
        private static MessageProcessor processor;

        static void Main()
        {
            var proxy = new WebProxy("134.209.217.230:8118");
            var botClient = new TelegramBotClient("660115843:AAGnddFLBWeam3Ko1TEu_j3-IRhUwuYBYM4", proxy);
            processor = new MessageProcessor(botClient);

            var me = botClient.GetMeAsync().GetAwaiter().GetResult();
            Console.WriteLine($"Bot {me.FirstName} is now online.");

            async void BotOnCallbackQuery(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
            {
                var query = callbackQueryEventArgs.CallbackQuery;
                var messageText = query.Data;
                var user = query.From.Username;
                var chatId = query?.Message.Chat.Id.ToString();
                await processor.Process(chatId, messageText, user);
                try {
                    await botClient.AnswerCallbackQueryAsync(query.Id);
                }
                catch (Exception exception) {
                    Console.WriteLine(exception);
                }
            }

            async void BotOnMessage(object sender, MessageEventArgs e)
            {
                var messageText = e.Message.Text;
                var user = e.Message.Chat.Username;
                var chatId = e.Message.Chat.Id.ToString();
                await processor.Process(chatId, messageText, user);
            }

            botClient.OnCallbackQuery += BotOnCallbackQuery;
            botClient.OnMessage += BotOnMessage;

            botClient.StartReceiving();
            Thread.Sleep(int.MaxValue);
        }
    }
}
