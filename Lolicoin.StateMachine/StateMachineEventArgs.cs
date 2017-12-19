using System;

namespace Lolicoin.StateMachine
{
    public class StateMachineEventArgs
    {
        public StateMachineEventArgs(
            string eventName,
            string eventInfo,
            StateMachineEventType eventType,
            string source,
            string target = "All")
        {
            EventName = eventName;
            EventInfo = eventInfo;
            EventType = eventType;
            Source    = source;
            Target    = target;
        }

        public string EventName                { get; }
        public string EventInfo                { get; }
        public DateTime TimeStamp              { get; }
        public string Source                   { get; }
        public string Target                   { get; }
        public StateMachineEventType EventType { get; }
    }

    public enum StateMachineEventType
    {
        System,
        Command,
        Notification,
        External
    }
}