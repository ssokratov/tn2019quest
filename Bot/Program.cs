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
            var proxy = new WebProxy("178.128.229.221:8080");
            var botClient = new TelegramBotClient("660115843:AAGnddFLBWeam3Ko1TEu_j3-IRhUwuYBYM4", proxy);
            processor = new MessageProcessor(botClient);

            var me = botClient.GetMeAsync().GetAwaiter().GetResult();
            Console.WriteLine($"Bot {me.FirstName} is now online.");

            async void BotOnCallbackQuery(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
            {
                var query = callbackQueryEventArgs.CallbackQuery;
                var answerMoMessage = query.Message;
                var messageText = query.Data;
                var user = query.From.Username ?? query.From.Id.ToString();
                var chatId = answerMoMessage.Chat.Id.ToString();
                var message = await processor.Process(chatId, messageText, user, answerMoMessage.MessageId);
                try {
                    if (!string.IsNullOrEmpty(message)) {
                        await botClient.AnswerCallbackQueryAsync(query.Id, message);
                    }
                }
                catch (Exception exception) {
                    Console.WriteLine(exception);
                }
            }

            async void BotOnMessage(object sender, MessageEventArgs e)
            {
                var messageText = e.Message.Text;
                var user = e.Message.From.Username ?? e.Message.From.Id.ToString();
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
