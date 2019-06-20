using System;
using Stateless;

namespace NewCellBot.Domain
{
    public class ConversationStateMachineDefinition
    {
        public static StateMachine<ConversationStates, ConversationTriggers> CreateStateMachine(
            Func<ConversationStates> stateAccessor,
            Action<ConversationStates> stateMutator,
            Action<StateMachine<ConversationStates, ConversationTriggers>.Transition> onTransitionAction,
            Action<ConversationStates, ConversationTriggers> onUnsupportedTransition)
        {
            var sm = new StateMachine<ConversationStates, ConversationTriggers>(stateAccessor, stateMutator);

            sm.Configure(ConversationStates.NotStarted)
                .Permit(ConversationTriggers.Start, ConversationStates.InGame)
                .PermitReentry(ConversationTriggers.Reset);

            sm.Configure(ConversationStates.InGame)
                .Permit(ConversationTriggers.FinishGame, ConversationStates.GameFinished)
                .Permit(ConversationTriggers.Reset, ConversationStates.NotStarted);

            sm.Configure(ConversationStates.GameFinished)
                .Permit(ConversationTriggers.Pay, ConversationStates.PayoutComplete)
                .Permit(ConversationTriggers.Reset, ConversationStates.NotStarted);

            sm.Configure(ConversationStates.PayoutComplete)
                .Permit(ConversationTriggers.Reset, ConversationStates.NotStarted);

            sm.OnTransitioned(onTransitionAction);
            sm.OnUnhandledTrigger(onUnsupportedTransition);

            return sm;
        }
    }
}