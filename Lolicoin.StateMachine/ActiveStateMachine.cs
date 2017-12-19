using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lolicoin.StateMachine
{
    public class ActiveStateMachine
    {
        private readonly State _initialState;
        private Task _queueWorkerTask;
        private ManualResetEvent _resumer;
        private CancellationTokenSource _tokenSource;

        public event EventHandler<StateMachineEventArgs> StateMachineEvent;

        public ActiveStateMachine(IDictionary<string, State> stateList, int queueCapacity)
        {
            StateList     = stateList;
            _initialState = new State("InitialState", null, null, null);
            TriggerQueue  = new BlockingCollection<string>(queueCapacity);

            InitStateMachine();
            RaiseStateMachineSystemEvent("StateMachine: Initialized", "System ready to start");
            StateMachineEngine = EngineState.Initialized;
        }

        public IDictionary<string, State> StateList    { get; }
        public BlockingCollection<string> TriggerQueue { get; }
        public State CurrentState                      { get; private set; }
        public State PreviousState                     { get; private set; }
        public EngineState StateMachineEngine          { get; private set; }

        public void Start()
        {
            _tokenSource       = new CancellationTokenSource();
            _queueWorkerTask   = Task.Factory.StartNew(QueueWorkerMethod, _tokenSource, TaskCreationOptions.LongRunning);
            StateMachineEngine = EngineState.Running;
            RaiseStateMachineSystemEvent("StateMachine: Started", "System running");
        }

        public void Pause()
        {
            StateMachineEngine = EngineState.Paused;
            _resumer.Reset();
            RaiseStateMachineSystemEvent("StateMachine: Paused", "System waiting");
        }

        public void Resume()
        {
            _resumer.Set();
            StateMachineEngine = EngineState.Running;
            RaiseStateMachineSystemEvent("StateMachine: Resumed", "System running");
        }

        public void Stop()
        {
            _tokenSource.Cancel();
            _queueWorkerTask.Wait();
            _queueWorkerTask.Dispose();
            StateMachineEngine = EngineState.Stopped;
            RaiseStateMachineSystemEvent("StateMachine: Stopped", "System execution stopped");
        }

        public void InitStateMachine()
        {
            PreviousState = _initialState;

            foreach (var state in StateList)
            {
                if (state.Value.IsDefaultState)
                {
                    CurrentState = state.Value;
                    RaiseStateMachineSystemCommand("OnInit", "StateMachineInitialized");
                }
            }

            _resumer = new ManualResetEvent(true);
        }

        private void EnterTrigger(string newTrigger)
        {
            try
            {
                TriggerQueue.Add(newTrigger);
            }
            catch (Exception e)
            {
                RaiseStateMachineSystemEvent("ActiveStateMachine - Error entering trigger", newTrigger + " - " + e);
            }

            RaiseStateMachineSystemEvent("ActiveStateMachine - Trigger entered", newTrigger);
        }

        private void QueueWorkerMethod(object dummy)
        {
            _resumer.WaitOne();

            try
            {
                foreach (var trigger in TriggerQueue.GetConsumingEnumerable())
                {
                    if (_tokenSource.IsCancellationRequested)
                    {
                        RaiseStateMachineSystemEvent("StateMachine : QueueWorker", "Processing canceled");
                        return;
                    }

                    foreach (var transition in CurrentState.StateTransitionList.Where(t => trigger == t.Value.Trigger))
                    {
                        ExecuteTransition(transition.Value);
                    }
                }
            }
            catch (Exception e)
            {
                RaiseStateMachineSystemEvent("StateMachine: QueueWorker", "Processing canceled. Exception: " + e);
                Start();
            }
        }

        public void InternalNotificationHandler(object sender, StateMachineEventArgs args)
        {
            if (args.EventName == "CompleteFailure")
            {
                RaiseStateMachineSystemCommand("CompleteFailure", args.EventInfo + " Device: " + args.Source);
                Stop();
            }
            else
            {
                EnterTrigger(args.EventName);
            }
        }

        protected virtual void ExecuteTransition(Transition transition)
        {
            if (CurrentState.StateName != transition.SourceStateName)
            {
                var message = $"Transition has wrong source state {transition.SourceStateName}, when system is in {CurrentState.StateName}.";
                RaiseStateMachineSystemEvent("StateMachine: Default guard execute transition", message);
                return;
            }

            if (transition.SourceStateName == transition.TargetStateName)
            {
                transition.TransitionActionList.ForEach(t => t.Execute());
                return;
            }

            CurrentState.ExitActions.ForEach(a => a.Execute());
            transition.GuardList.ForEach(g => g.Execute());

            var info = transition.GuardList.Count + " guard actions executed.";
            RaiseStateMachineSystemEvent("StateMachine: ExecuteTransition", info);

            transition.TransitionActionList.ForEach(t => t.Execute());

            info = transition.TransitionActionList.Count + " transition actions executed.";
            RaiseStateMachineSystemEvent("StateMachine: Begin state change.", info);

            var targetState = GetStateFromStateList(transition.TargetStateName);

            PreviousState = CurrentState;
            CurrentState  = targetState;

            CurrentState.EntryActions.ForEach(a => a.Execute());

            RaiseStateMachineSystemEvent("StateMachine: State change completed successfully.", "Previous state: " +
                PreviousState.StateName + " - New state = " + CurrentState.StateName);
        }

        private State GetStateFromStateList(string targetStateName) => StateList[targetStateName];

        private void RaiseStateMachineSystemEvent(string eventName, string eventInfo) =>
            StateMachineEvent?.Invoke(this, new StateMachineEventArgs(eventName, eventInfo, StateMachineEventType.System, "StateMachine"));

        private void RaiseStateMachineSystemCommand(string eventName, string eventInfo) =>
            StateMachineEvent?.Invoke(this, new StateMachineEventArgs(eventName, eventInfo, StateMachineEventType.Command, "StateMachine"));
    }

    public enum EngineState
    {
        Running,
        Stopped,
        Paused,
        Initialized
    }
}