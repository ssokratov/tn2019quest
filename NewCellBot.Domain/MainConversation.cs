using System;
using System.Collections.Generic;
using Stateless;
using Telegram.Bot.Types;

namespace NewCellBot.Domain
{
    public class MainConversation
    {
        private StateMachine<ConversationStates, ConversationTriggers> _stateMachine;
        private List<Message> _messages = new List<Message>();

        public MainConversation(ConversationStates conversationState)
        {
            ConversationState = conversationState;

            _stateMachine =
                ConversationStateMachineDefinition.CreateStateMachine(
                    () => ConversationState,
                    s => ConversationState = s,
                    On,
                    OnUnsupported);
        }

        public IEnumerable<Message> Messages
        {
            get => _messages;
        }

        public ConversationStates ConversationState { get; private set; } = ConversationStates.NotStarted;

        public void Start(long userId)
        {
            if (userId != UserIds.Nastya && userId != UserIds.Toshik)
            {
                _messages.Add(new Message {Text = "Ты, похоже, пидорок"});
            }

            _stateMachine.Fire(ConversationTriggers.Start);
        }

        public void FinishGame()
        {
            _stateMachine.Fire(ConversationTriggers.FinishGame);
        }

        public void Pay()
        {
            _stateMachine.Fire(ConversationTriggers.Pay);
        }

        public void Reset()
        {
            _stateMachine.Fire(ConversationTriggers.Reset);
        }

        private void On(StateMachine<ConversationStates,ConversationTriggers>.Transition transition)
        {
            switch (transition.Destination)
            {
                case ConversationStates.InGame:
                    _messages.Add(new Message {Text = "Проходи садись"});
                    break;
                case ConversationStates.GameFinished:
                    _messages.Add(new Message {Text = "Игра окончена"});
                    break;
                case ConversationStates.PayoutComplete:
                    _messages.Add(new Message {Text = "Уплочено"});
                    break;
                case ConversationStates.NotStarted:
                    _messages.Add(new Message {Text = "Сброшено"});
                    break;
            }
        }
        private void OnUnsupported(ConversationStates state, ConversationTriggers trigger)
        {
            _messages.Add(new Message {Text = $"Я пока не умею {trigger} в {state}"});
        }
    }
}