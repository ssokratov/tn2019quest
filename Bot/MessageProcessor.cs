using System;
using System.Collections.Concurrent;
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
    private readonly ConcurrentDictionary<string, SemaphoreSlim> synchronizerDict = new ConcurrentDictionary<string, SemaphoreSlim>();

    public async Task<string> Process(string chatId,
        string messageText,
        string user,
        int? answerToMessageId = null)
    {
        var synchronizer = synchronizerDict.ContainsKey(chatId)
            ? synchronizerDict[chatId]
            : (synchronizerDict[chatId] = new SemaphoreSlim(1, 1));

        if (!await synchronizer.WaitAsync(1000)) {
            Console.WriteLine("Error: can't take lock.");
            return "";
        }

        try {
            // initialize new state if new player, or on "reset" command
            if (stateManager.GetState(chatId) == null || messageText.StartsWith("/reset")) {
                stateManager.SetState(chatId, new BotState {
                    QuestState = new QuestService(NewCellQuest.Map,
                        NewCellQuest.GetStartingInventory(),
                        NewCellQuest.GetStartingJournal(),
                        NewCellQuest.GetDialogs())
                        .State
                });
            }
            else if (stateManager.GetState(chatId) == null || messageText.StartsWith("/nastya")) {
                var questState = new QuestService(NewCellQuest.Map,
                        NewCellQuest.GetStartingInventory(),
                        NewCellQuest.GetStartingJournal(),
                        NewCellQuest.GetDialogs())
                    .State;
                questState.OpenDialogName = Dialog.ZagsEnd;
                questState.Inventory = questState.Inventory.Give(Item.Glasses);
                stateManager.SetState(chatId, new BotState {
                    QuestState = questState
                });
            }
            // reset all hashcodes to send all messages again if "play" commend is received
            else if (messageText.StartsWith("/play")) {
                var oldState = stateManager.GetState(chatId);
                oldState.PreviousMessageHash = -1;
                stateManager.SetState(chatId, oldState);
            }

            var botState = stateManager.GetState(chatId);
            var questService = new QuestService(NewCellQuest.GetDialogs(), botState.QuestState);

            if (messageText != null) {
                Console.WriteLine($"Received a text message in chat {chatId} from {user}: \n {messageText}");
            }

            var userValidation = questService.CanPlay(user);
            if (!userValidation.CanPlay) {
                return userValidation.Reason;
            }

            var currentMove = questService.ProcessAnswer((messageText?.IsSmile() == true)
                ? messageText.FromSmile().ToString()
                : messageText);
            var answerButtons = RenderButtons(currentMove);
            var message = currentMove.Photo != null
                ? await SendMediaMessage(chatId, currentMove, botState, answerButtons, answerToMessageId)
                : await SendTextMessage(chatId, currentMove, botState, answerButtons, answerToMessageId);

            if (message != null) {
                botState.QuestState = questService.State;
                stateManager.SetState(chatId, botState);
            }
        }
        catch (Exception exception) {
            Console.WriteLine(exception);
        }
        finally {
            if (synchronizer.CurrentCount == 0) {
                synchronizer.Release();
            }
            else {
                Console.WriteLine("Error: Synchronizer count is unexpected. Expected: 0, actual: " + synchronizer.CurrentCount);
            }
        }
        return "";
    }

    private async Task<Message> SendTextMessage(string chatId,
        VisibleState currentMove,
        BotState botState,
        InlineKeyboardMarkup answerButtons,
        int? answerToMessageId = null)
    {
        var response = currentMove.Message;
        Message message = null;
        if (response != null && response.GetHashCode() != botState.PreviousMessageHash) {
            if (answerToMessageId.HasValue && botState.PreviousMessageIsText) {
                try {
                    message = await botClient.EditMessageTextAsync(
                        chatId: chatId,
                        messageId: answerToMessageId.Value,
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
                var messageToDelete = botState.PreviousMessageIsText
                    ? botState.PreviousMessageId
                    : answerToMessageId;
                if (messageToDelete.HasValue) {
                    try {
                        await botClient.DeleteMessageAsync(
                            chatId: chatId,
                            messageId: messageToDelete.Value
                        );
                    }
                    catch (Exception exception) {
                        Console.WriteLine(exception);
                    }
                }
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

            botState.PreviousMessageIsText = true;
            botState.PreviousMessageId = message?.MessageId;
            botState.PreviousMessageHash = response.GetHashCode();
        }

        return message;
    }

    private async Task<Message> SendMediaMessage(string chatId,
        VisibleState currentMove, 
        BotState botState,
        InlineKeyboardMarkup answerButtons,
        int? answerToMessageId = null)
    {
        var response = currentMove.Message;
        var photo = currentMove.Photo;
        Message message = null;
        if (photo != null && photo.GetHashCode() != botState.PreviousMessageHash) {
            var filePath = photo.Split(';').FirstOrDefault();
            var fileId = photo.Split(';').LastOrDefault();

            if (answerToMessageId.HasValue) {
                try {
                    await botClient.DeleteMessageAsync(
                        chatId: chatId,
                        messageId: answerToMessageId.Value
                    );
                }
                catch (Exception exception) {
                    Console.WriteLine(exception);
                }
            }

            if (!string.IsNullOrEmpty(fileId) && fileId != filePath) {
                try {
                    message = await botClient.SendPhotoAsync(
                        chatId: chatId,
                        photo: new InputMedia(fileId),
                        replyMarkup: answerButtons,
                        caption: response
                    );
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                }
            }

            if (message == null && filePath != null) {
                try {
                    using (var fileStream = new FileStream(filePath, FileMode.Open)) {
                        message = await botClient.SendPhotoAsync(
                            chatId: chatId,
                            photo: new InputMedia(fileStream, photo),
                            replyMarkup: answerButtons,
                            caption: response
                        );
                        Console.WriteLine("FILE UPLOADED. " + string.Join("\n", message.Photo.Select(p =>
                                              $"W: {p.Width}, H: {p.Height}, ID:{p.FileId}")));
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                }
            }

            botState.PreviousMessageIsText = false;
            botState.PreviousMessageId = message?.MessageId;
            botState.PreviousMessageHash = photo.GetHashCode();
        }

        return message;
    }

    private static InlineKeyboardMarkup RenderButtons(VisibleState currentMove)
    {
        InlineKeyboardMarkup answerButtons = null;
        if (currentMove.Answers?.Length > 0) {
            List<InlineKeyboardButton> groupBy3 = null;
            List<InlineKeyboardButton> groupBy2 = null;
            var buttonCategories = new List<List<InlineKeyboardButton>>();
            foreach (var answer in currentMove.Answers) {
                var len = answer.Length;
                if (len == 1) {
                    if (groupBy3 == null || groupBy3.Count >= 3) {
                        groupBy3 = new List<InlineKeyboardButton>();
                        buttonCategories.Add(groupBy3);
                    }
                    groupBy3.Add(InlineKeyboardButton.WithCallbackData(answer[0].ToSmile()));
                }
                else if (len < 12) {
                    if (groupBy2 == null || groupBy2.Count >= 2) {
                        groupBy2 = new List<InlineKeyboardButton>();
                        buttonCategories.Add(groupBy2);
                    }
                    groupBy2.Add(InlineKeyboardButton.WithCallbackData(answer));
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