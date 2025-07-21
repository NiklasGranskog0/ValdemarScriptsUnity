using System;

namespace Assets.Scripts.Interfaces
{
    public interface IEvent { }
    
    internal interface IEventBinding<T>
    {
        public Action<T> OnEvent { get; set; }
        public Action OnEventNoArgs { get; set; }
    }
    
    public class EventBinding<T> : IEventBinding<T> where T : IEvent
    {
        private Action<T> m_OnEvent = _ => {};
        private Action m_OnEventNoArgs = () => {};

        Action<T> IEventBinding<T>.OnEvent { get => m_OnEvent; set => m_OnEvent = value; }
        Action IEventBinding<T>.OnEventNoArgs { get => m_OnEventNoArgs; set => m_OnEventNoArgs = value; }

        public EventBinding(Action<T> onEvent) => m_OnEvent = onEvent;
        public EventBinding(Action onEventNoArgs) => m_OnEventNoArgs = onEventNoArgs;

        public void Add(Action<T> onEvent) => m_OnEvent += onEvent;
        public void Remove(Action<T> onEvent) => m_OnEvent -= onEvent;
        
        public void Add(Action onEventNoArgs) => m_OnEventNoArgs += onEventNoArgs;
        public void Remove(Action onEventNoArgs) => m_OnEventNoArgs -= onEventNoArgs;
    }
}