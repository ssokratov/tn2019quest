using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bot;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

public class MessageProcessor
{
    private readonly ITelegramBotClient botClient;
    private readonly QuestStateManager stateManager;
    private readonly SemaphoreSlim synchronizer = new SemaphoreSlim(1, 1);

    public async Task Process(string chatId, string messageText, string user)
    {
        await synchronizer.WaitAsync();
        try {
             // initialize new state if new player, or on "reset" command
            if (stateManager.GetState(chatId) == null || messageText.StartsWith("/reset")) {
                stateManager.SetState(chatId, new QuestState {
                    Service = ToshikQuest.Initialize()
                });
            }
            // reset all hashcodes to send all messages again if "play" commend is received
            else if (messageText.StartsWith("/play")) {
                var oldState = stateManager.GetState(chatId);
                oldState.PreviousMediaMessageHash = -1;
                oldState.PreviousMessageHash = -1;
                stateManager.SetState(chatId, oldState);
            }

            var questState = stateManager.GetState(chatId);
            var questService = questState.Service;

            if (messageText != null) {
                Console.WriteLine($"Received a text message in chat {chatId} from {user}: \n {messageText}");
            }

            var currentMove = questService.ProcessAnswer((messageText?.IsSmile() == true)
                ? messageText.FromSmile().ToString()
                : messageText);
            var answerButtons = RenderButtons(currentMove);
            var mediaMessage = await SendMediaMessage(chatId, currentMove, questState, answerButtons);
            await SendTextMessage(chatId, currentMove, questState, mediaMessage == null ? answerButtons : null);
        }
        finally {
            synchronizer.Release();
        }
    }

    private async Task<Message> SendTextMessage(string chatId, VisibleState currentMove, QuestState questState,
        InlineKeyboardMarkup answerButtons)
    {
        var response = currentMove.Message;
        Message message = null;
        if (response != null && response.GetHashCode() != questState.PreviousMessageHash) {
            if (questState.PreviousMessageId.HasValue) {
                try {
                    message = await botClient.EditMessageTextAsync(
                        chatId: chatId,
                        messageId: questState.PreviousMessageId.Value,
                        text: response,
                        replyMarkup: answerButtons,
                        parseMode: ParseMode.Markdown
                    );
                }
                catch (Exception exception) {
                    Console.WriteLine(exception);
                }
            }

            if (message == null) {
                try {
                    message = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: response,
                        replyMarkup: answerButtons,
                        parseMode: ParseMode.Markdown
                    );
                }
                catch (Exception exception) {
                    Console.WriteLine(exception);
                }
            }

            questState.PreviousMessageId = message?.MessageId;
            questState.PreviousMessageHash = response.GetHashCode();
        }

        return message;
    }

    private async Task<Message> SendMediaMessage(string chatId,
        VisibleState currentMove, 
        QuestState questState,
        InlineKeyboardMarkup answerButtons)
    {
        var photo = currentMove.Photo;
        Message mediaMessage = null;
        if (photo == null && questState.PreviousMediaMessageId.HasValue) {
            try {
                await botClient.DeleteMessageAsync(
                    chatId: chatId,
                    messageId: questState.PreviousMediaMessageId.Value
                );
                questState.PreviousMediaMessageHash = null;
                questState.PreviousMediaMessageId = null;
            }
            catch (Exception exception) {
                Console.WriteLine(exception);
            }
        }

        if (photo != null && photo.GetHashCode() != questState.PreviousMediaMessageHash) {
            var filePath = photo.Split(new[] { "***" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            var fileId = photo.Split(new[] { "***" }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();

            if (fileId != null) {
                try {
                    mediaMessage = await botClient.SendPhotoAsync(
                        chatId: chatId,
                        photo: new InputMedia(fileId),
                        replyMarkup: answerButtons
                    );
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                }
            }

            if (mediaMessage == null && filePath != null) {
                try {
                    using (var fileStream = new FileStream(filePath, FileMode.Open)) {
                        mediaMessage = await botClient.SendPhotoAsync(
                            chatId: chatId,
                            photo: new InputMedia(fileStream, photo),
                            replyMarkup: answerButtons
                        );
                        Console.WriteLine("FILE UPLOADED. " + string.Join("\n",
                                              mediaMessage.Photo.Select(p =>
                                                  $"W: {p.Width}, H: {p.Height}, ID:{p.FileId}")));
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                }
            }

            questState.PreviousMediaMessageHash = photo.GetHashCode();
            questState.PreviousMediaMessageId = mediaMessage?.MessageId;
        }

        return mediaMessage;
    }

    private static InlineKeyboardMarkup RenderButtons(VisibleState currentMove)
    {
        InlineKeyboardMarkup answerButtons = null;
        if (currentMove.Answers?.Length > 0) {
            var buttonCategories = new List<List<InlineKeyboardButton>> { new List<InlineKeyboardButton>() };
            foreach (var answer in currentMove.Answers) {
                var len = answer.Length;
                if (len == 1 && answer[0] == Directions.Up) {
                    buttonCategories[0].Add(InlineKeyboardButton.WithCallbackData(answer[0].ToSmile()));
                }
                else if (len == 1) {
                    if (buttonCategories.Count < 2) {
                        buttonCategories.Add(new List<InlineKeyboardButton>());
                    }

                    buttonCategories[1].Add(InlineKeyboardButton.WithCallbackData(answer[0].ToSmile()));
                }
                else {
                    buttonCategories.Add(new List<InlineKeyboardButton> {
                        InlineKeyboardButton.WithCallbackData(answer)
                    });
                }
            }

            answerButtons = new InlineKeyboardMarkup(buttonCategories);
        }

        return answerButtons;
    }

    public MessageProcessor(ITelegramBotClient botClient)
    {
        this.botClient = botClient;
        stateManager = new QuestStateManager();
    }
}