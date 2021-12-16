using Discord;
using System.Collections.Immutable;

namespace MASZ.Utils
{
    public class AsyncEvent<T>
        where T : class
    {
        private readonly object _subLock = new ();
        internal ImmutableArray<T> _subscriptions;

        public bool HasSubscribers => _subscriptions.Length != 0;
        public IReadOnlyList<T> Subscriptions => _subscriptions;

        public AsyncEvent()
        {
            _subscriptions = ImmutableArray.Create<T>();
        }

        public void Add(T subscriber)
        {
            subscriber.NotNull(nameof(subscriber));
            lock (_subLock)
                _subscriptions = _subscriptions.Add(subscriber);
        }
        public void Remove(T subscriber)
        {
            subscriber.NotNull(nameof(subscriber));
            lock (_subLock)
                _subscriptions = _subscriptions.Remove(subscriber);
        }
    }

    internal static class EventExtensions
    {
        public static void NotNull<T>(this T obj, string name, string msg = null) where T : class { if (obj == null) throw CreateNotNullException(name, msg); }

        public static void NotNull<T>(this Optional<T> obj, string name, string msg = null) where T : class { if (obj.IsSpecified && obj.Value == null) throw CreateNotNullException(name, msg); }

        private static ArgumentNullException CreateNotNullException(string name, string msg)
        {
            if (msg == null) return new ArgumentNullException(paramName: name);
            else return new ArgumentNullException(paramName: name, message: msg);
        }

        public static async Task InvokeAsync(this AsyncEvent<Func<Task>> eventHandler)
        {
            var subscribers = eventHandler.Subscriptions;
            for (int i = 0; i < subscribers.Count; i++)
                await subscribers[i].Invoke().ConfigureAwait(false);
        }
        public static async Task InvokeAsync<T>(this AsyncEvent<Func<T, Task>> eventHandler, T arg)
        {
            var subscribers = eventHandler.Subscriptions;
            for (int i = 0; i < subscribers.Count; i++)
                await subscribers[i].Invoke(arg).ConfigureAwait(false);
        }
        public static async Task InvokeAsync<T1, T2>(this AsyncEvent<Func<T1, T2, Task>> eventHandler, T1 arg1, T2 arg2)
        {
            var subscribers = eventHandler.Subscriptions;
            for (int i = 0; i < subscribers.Count; i++)
                await subscribers[i].Invoke(arg1, arg2).ConfigureAwait(false);
        }
        public static async Task InvokeAsync<T1, T2, T3>(this AsyncEvent<Func<T1, T2, T3, Task>> eventHandler, T1 arg1, T2 arg2, T3 arg3)
        {
            var subscribers = eventHandler.Subscriptions;
            for (int i = 0; i < subscribers.Count; i++)
                await subscribers[i].Invoke(arg1, arg2, arg3).ConfigureAwait(false);
        }
        public static async Task InvokeAsync<T1, T2, T3, T4>(this AsyncEvent<Func<T1, T2, T3, T4, Task>> eventHandler, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            var subscribers = eventHandler.Subscriptions;
            for (int i = 0; i < subscribers.Count; i++)
                await subscribers[i].Invoke(arg1, arg2, arg3, arg4).ConfigureAwait(false);
        }
        public static async Task InvokeAsync<T1, T2, T3, T4, T5>(this AsyncEvent<Func<T1, T2, T3, T4, T5, Task>> eventHandler, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            var subscribers = eventHandler.Subscriptions;
            for (int i = 0; i < subscribers.Count; i++)
                await subscribers[i].Invoke(arg1, arg2, arg3, arg4, arg5).ConfigureAwait(false);
        }
    }
}