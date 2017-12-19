using System;
using System.Collections.Generic;

namespace Lolicoin.StateMachine
{
    public class State
    {
        public State(
            string stateName,
            Dictionary<string, Transition> transitionList,
            List<StateMachineAction> entryActions,
            List<StateMachineAction> exitActions,
            bool defaultState = false)
        {
            StateName           = stateName;
            StateTransitionList = transitionList;
            IsDefaultState      = defaultState;
            EntryActions        = entryActions;
            ExitActions         = exitActions;
        }

        public string StateName                                   { get; }
        public Dictionary<string, Transition> StateTransitionList { get; }
        public List<StateMachineAction> EntryActions              { get; }
        public List<StateMachineAction> ExitActions               { get; }
        public bool IsDefaultState                                { get; }
    }
}