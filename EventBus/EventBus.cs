using System.Collections.Generic;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Framework.EventBus
{
    public static class EventBus<T> where T : IEvent
    {
        private static readonly HashSet<IEventBinding<T>> s_bindings = new();

        public static void Register(EventBinding<T> binding) => s_bindings.Add(binding);
        public static void DeRegister(EventBinding<T> binding) => s_bindings.Remove(binding);

        public static void Raise(T @event)
        {
            foreach (var binding in s_bindings)
            {
                binding.OnEvent.Invoke(@event);
                binding.OnEventNoArgs.Invoke();
            }
        }

        private static void Clear()
        {
            Debug.Log($"Clearing {typeof(T).Name} bindings");
            s_bindings.Clear();
        }
    }
}