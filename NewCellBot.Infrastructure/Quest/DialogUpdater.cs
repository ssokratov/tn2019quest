using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bot;
using Microsoft.Extensions.Logging;
using NewCellBot.Domain;
using NewCellBot.Domain.Quest.Model;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace NewCellBot.Infrastructure.Quest
{
    public class DialogUpdater
    {
        private readonly BotWrapper _botWrapper;
        private readonly ILogger<DialogUpdater> _logger;

        public DialogUpdater(BotWrapper botWrapper, ILogger<DialogUpdater> logger)
        {
            _botWrapper = botWrapper;
            _logger = logger;
        }

        public async Task<Message> SendTextMessage(
            long chatId,
            VisibleState currentMove,
            ChatState chatState,
            InlineKeyboardMarkup answerButtons,
            int? answerToMessageId = null)
        {
            var response = currentMove.Message;
            Message message = null;
            if (response != null && response.GetHashCode() != chatState.PreviousMessageHash)
            {
                if (answerToMessageId.HasValue && chatState.PreviousMessageIsText)
                {
                    try
                    {
                        message = await _botWrapper.Client.EditMessageTextAsync(
                            chatId: chatId,
                            messageId: answerToMessageId.Value,
                            text: response,
                            replyMarkup: answerButtons,
                            parseMode: ParseMode.Markdown
                        );
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(exception, "Cannot edit message");
                        throw;
                    }
                }

                if (message == null)
                {
                    var messageToDelete = chatState.PreviousMessageIsText
                        ? chatState.PreviousMessageId
                        : answerToMessageId;
                    if (messageToDelete.HasValue)
                    {
                        try
                        {
                            await _botWrapper.Client.DeleteMessageAsync(
                                chatId: chatId,
                                messageId: messageToDelete.Value
                            );
                        }
                        catch (Exception exception)
                        {
                            _logger.LogError(exception, "Cannot delete message");
                            throw;
                        }
                    }

                    try
                    {
                        message = await _botWrapper.Client.SendTextMessageAsync(
                            chatId: chatId,
                            text: response,
                            replyMarkup: answerButtons,
                            parseMode: ParseMode.Markdown
                        );
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(exception, "Cannot send message");
                        throw;
                    }
                }

                chatState.PreviousMessageIsText = true;
                chatState.PreviousMessageId = message?.MessageId;
                chatState.PreviousMessageHash = response.GetHashCode();
            }

            return message;
        }

        public async Task<Message> SendMediaMessage(long chatId,
            VisibleState currentMove,
            ChatState botState,
            InlineKeyboardMarkup answerButtons,
            int? answerToMessageId = null)
        {
            var response = currentMove.Message;
            var photo = currentMove.Photo;
            Message message = null;
            if (photo != null && photo.GetHashCode() != botState.PreviousMessageHash)
            {
                var filePath = photo.Split(';').FirstOrDefault();
                var fileId = photo.Split(';').LastOrDefault();

                if (answerToMessageId.HasValue)
                {
                    try
                    {
                        await _botWrapper.Client.DeleteMessageAsync(
                            chatId: chatId,
                            messageId: answerToMessageId.Value
                        );
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(exception, "Cannot delete message");
                        throw;
                    }
                }

                if (!string.IsNullOrEmpty(fileId) && fileId != filePath)
                {
                    try
                    {
                        message = await _botWrapper.Client.SendPhotoAsync(
                            chatId: chatId,
                            photo: new InputMedia(fileId),
                            replyMarkup: answerButtons,
                            caption: response
                        );
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Cannot send photo");
                    }
                }

                if (message == null && filePath != null)
                {
                    try
                    {
                        using (var fileStream = new FileStream(filePath, FileMode.Open))
                        {
                            message = await _botWrapper.Client.SendPhotoAsync(
                                chatId: chatId,
                                photo: new InputMedia(fileStream, photo),
                                replyMarkup: answerButtons,
                                caption: response
                            );
                            _logger.LogWarning(
                                $"FILE '{filePath}' UPLOADED. " + string.Join("\n", message.Photo.Select(p =>
                                    $"W: {p.Width}, H: {p.Height}, ID:{p.FileId}")));
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Cannot upload photo");
                    }
                }

                botState.PreviousMessageIsText = false;
                botState.PreviousMessageId = message?.MessageId;
                botState.PreviousMessageHash = photo.GetHashCode();
            }

            return message;
        }

        public InlineKeyboardMarkup GetButtons(VisibleState currentMove)
        {
            InlineKeyboardMarkup answerButtons = null;
            if (currentMove.Answers?.Length > 0)
            {
                List<InlineKeyboardButton> groupBy3 = null;
                List<InlineKeyboardButton> groupBy2 = null;
                var buttonCategories = new List<List<InlineKeyboardButton>>();
                foreach (var answer in currentMove.Answers)
                {
                    var len = answer.Length;
                    if (len == 1)
                    {
                        if (groupBy3 == null || groupBy3.Count >= 3)
                        {
                            groupBy3 = new List<InlineKeyboardButton>();
                            buttonCategories.Add(groupBy3);
                        }
                        groupBy3.Add(InlineKeyboardButton.WithCallbackData(answer[0].ToSmile()));
                    }
                    else if (len < 12)
                    {
                        if (groupBy2 == null || groupBy2.Count >= 2)
                        {
                            groupBy2 = new List<InlineKeyboardButton>();
                            buttonCategories.Add(groupBy2);
                        }
                        groupBy2.Add(InlineKeyboardButton.WithCallbackData(answer));
                    }
                    else
                    {
                        buttonCategories.Add(new List<InlineKeyboardButton> {
                            InlineKeyboardButton.WithCallbackData(answer)
                        });
                    }
                }

                answerButtons = new InlineKeyboardMarkup(buttonCategories);
            }

            return answerButtons;
        }
    }
}