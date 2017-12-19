using System;

namespace Lolicoin.StateMachine
{
    public abstract class StateMachineAction
    {
        private readonly Action _method;

        protected StateMachineAction(string name, Action method)
        {
            Name    = name;
            _method = method;
        }

        public string Name { get; }

        public void Execute() => _method.Invoke();
    }
}