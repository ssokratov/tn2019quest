using System;
using System.Data;
using System.Threading.Tasks;
using Bot;
using Microsoft.Extensions.Logging;
using NewCellBot.Domain;
using NewCellBot.Domain.Quest;
using NewCellBot.Infrastructure;
using NewCellBot.Infrastructure.Quest;
using Newtonsoft.Json;
using Polly;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace NewCellBot.Application
{
    public class UpdateService
    {
        private readonly StateStorage _stateStorage;
        private readonly ILogger<UpdateService> _logger;
        private readonly BotWrapper _botWrapper;
        private readonly DialogUpdater _dialogUpdater;

        private readonly AsyncPolicy<string> _concurrencyRetryPolicy;
        private readonly Random _jitterer;

        public UpdateService(
            StateStorage stateStorage,
            ILogger<UpdateService> logger,
            BotWrapper botWrapper,
            DialogUpdater dialogUpdater)
        {
            _stateStorage = stateStorage;
            _logger = logger;
            _botWrapper = botWrapper;
            _dialogUpdater = dialogUpdater;

            _jitterer = new Random();

            _concurrencyRetryPolicy = Policy<string>
                .Handle<MessageIsNotModifiedException>()
                .Or<ApiRequestException>(e => e.Message.ToLowerInvariant().Contains("message"))
                .Or<DBConcurrencyException>()
                .WaitAndRetryAsync(
                    5,
                    i => TimeSpan.FromSeconds(i).Add(TimeSpan.FromMilliseconds(_jitterer.Next(0, 1000))),
                    (result, span) => _logger.LogWarning(result.Exception, "Retrying concurrency error"));
        }

        public async Task HandleAsync(Update update)
        {
            try
            {
                if (update.Type == UpdateType.CallbackQuery)
                {
                    await BotOnCallbackQuery(update.CallbackQuery);
                }
                else if (update.Type == UpdateType.Message)
                {
                    var message = update.Message;
                    var chatId = message.Chat.Id;
                    var user = message.From.Username ?? message.From.Id.ToString();

                    _logger.LogInformation("Received Message from user {0}, chat {2}", user, chatId);

                    if (message.Type == MessageType.Text)
                    {
                        BotOnMessage(message);
                    }
                }
                else
                {
                    _logger.LogWarning("Dropping update: \n" + JsonConvert.SerializeObject(update));
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error handling update");
            }
        }

        private async Task BotOnCallbackQuery(CallbackQuery query)
        {
            var callbackMessage = query.Message;
            var messageText = query.Data;
            var user = query.From.Username ?? query.From.Id.ToString();
            var chatId = callbackMessage.Chat.Id;


            var message = await _concurrencyRetryPolicy.ExecuteAsync(async () => await Process(chatId, messageText, user, callbackMessage.MessageId));

            try
            {
                if (!string.IsNullOrEmpty(message))
                {
                    await _botWrapper.Client.AnswerCallbackQueryAsync(query.Id, message);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Cannot answer callback query");
                throw;
            }
        }

        private async void BotOnMessage(Message message)
        {
            var messageText = message.Text;
            var user = message.From.Username ?? message.From.Id.ToString();
            var chatId = message.Chat.Id;

            await _concurrencyRetryPolicy.ExecuteAsync(async () => await Process(chatId, messageText, user));
        }

        public async Task<string> Process(
            long chatId,
            string messageText,
            string user,
            int? answerToMessageId = null)
        {
            try
            {
                var state = await LoadState(chatId, messageText);

                var questService = new QuestService(NewCellQuest.GetDialogs(), state.QuestState);

                if (messageText != null)
                {
                    _logger.LogInformation($"Received a text message in chat {chatId} from {user}: \n {messageText}");
                }

                var userValidation = questService.CanPlay(user);
                if (!userValidation.CanPlay)
                {
                    return userValidation.Reason;
                }

                var currentMove = questService.ProcessAnswer((messageText?.IsSmile() == true)
                    ? messageText.FromSmile().ToString()
                    : messageText);
                var answerButtons = _dialogUpdater.GetButtons(currentMove);

                await Task.Delay(_jitterer.Next(0, 500));

                var message = currentMove.Photo != null
                    ? await _dialogUpdater.SendMediaMessage(chatId, currentMove, state.ChatState, answerButtons, answerToMessageId)
                    : await _dialogUpdater.SendTextMessage(chatId, currentMove, state.ChatState, answerButtons, answerToMessageId);

                if (message != null)
                {
                    state.QuestState = questService.State;
                    await _stateStorage.StoreStateAsync(state);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error handling message");
                throw;
            }

            return "";
        }

        private async Task<BotState> LoadState(long chatId, string messageText)
        {
            // initialize new state if new player, or on "reset" command
            var state = await _stateStorage.GetStateAsync(chatId);

            if (state == null || messageText.StartsWith("/reset"))
            {
                state = new BotState
                {
                    QuestState = new QuestService(NewCellQuest.Map,
                            NewCellQuest.GetStartingInventory(),
                            NewCellQuest.GetStartingJournal(),
                            NewCellQuest.GetDialogs())
                        .State,
                    ChatId = chatId,
                    ChatState = new ChatState()
                };
                await _stateStorage.StoreStateAsync(state);
            }
            else if (messageText.StartsWith("/nastya"))
            {
                var questState = new QuestService(NewCellQuest.Map,
                        NewCellQuest.GetStartingInventory(),
                        NewCellQuest.GetStartingJournal(),
                        NewCellQuest.GetDialogs())
                    .State;
                questState.OpenDialogName = Dialog.ZagsEnd;
                questState.Inventory = questState.Inventory.Give(Item.Glasses);
                state = new BotState
                {
                    ChatId = chatId,
                    QuestState = questState,
                    ChatState = new ChatState()
                };
                await _stateStorage.StoreStateAsync(state);
            }
            // reset all hashcodes to send all messages again if "play" commend is received
            else if (messageText.StartsWith("/play"))
            {
                var oldState = state;
                oldState.ChatState.PreviousMessageHash = -1;
                await _stateStorage.StoreStateAsync(oldState);
            }

            return state;
        }
    }
}