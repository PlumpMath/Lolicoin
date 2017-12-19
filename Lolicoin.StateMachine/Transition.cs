using System.Collections.Generic;

namespace Lolicoin.StateMachine
{
    public abstract class Transition
    {
        protected Transition(
            string name,
            string sourceState,
            string targetState,
            List<StateMachineAction> guardList,
            List<StateMachineAction> transitionActionList,
            string trigger)
        {
            Name                 = name;
            SourceStateName      = sourceState;
            TargetStateName      = targetState;
            GuardList            = guardList;
            TransitionActionList = transitionActionList;
            Trigger              = trigger;
        }

        public string Name                                   { get; }
        public string SourceStateName                        { get; }
        public string TargetStateName                        { get; }
        public List<StateMachineAction> GuardList            { get; }
        public List<StateMachineAction> TransitionActionList { get; }
        public string Trigger                                { get; }
    }
}