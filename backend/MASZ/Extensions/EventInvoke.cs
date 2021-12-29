using Discord;
using MASZ.Utils;

namespace MASZ.Extensions
{
    internal static class EventInvoke
    {
        public static void NotNull<T>(this T obj, string name, string msg = null) where T : class { if (obj == null) throw CreateNotNullException(name, msg); }

        public static void NotNull<T>(this Optional<T> obj, string name, string msg = null) where T : class { if (obj.IsSpecified && obj.Value == null) throw CreateNotNullException(name, msg); }

        private static ArgumentNullException CreateNotNullException(string name, string msg)
        {
            if (msg == null) return new ArgumentNullException(paramName: name);
            else return new ArgumentNullException(paramName: name, message: msg);
        }

        public static void InvokeAsync(this AsyncEvent<Func<Task>> eventHandler)
        {
            var subscribers = eventHandler.Subscriptions;
            foreach (var subscription in subscribers)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await subscription.Invoke().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Something went wrong while executing subscription {subscription.Target}/{subscription.Method.Name} of {eventHandler.GetName()}.");
                        Console.WriteLine(ex);
                    }
                });
            }
        }
        public static void InvokeAsync<T>(this AsyncEvent<Func<T, Task>> eventHandler, T arg)
        {
            var subscribers = eventHandler.Subscriptions;
            foreach (var subscription in subscribers)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await subscription.Invoke(arg).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Something went wrong while executing subscription {subscription.Target}/{subscription.Method.Name} of {eventHandler.GetName()}.");
                        Console.WriteLine(ex);
                    }
                });
            }
        }
        public static void InvokeAsync<T1, T2>(this AsyncEvent<Func<T1, T2, Task>> eventHandler, T1 arg1, T2 arg2)
        {
            var subscribers = eventHandler.Subscriptions;
            foreach (var subscription in subscribers)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await subscription.Invoke(arg1, arg2).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Something went wrong while executing subscription {subscription.Target}/{subscription.Method.Name} of {eventHandler.GetName()}.");
                        Console.WriteLine(ex);
                    }
                });
            }
        }
        public static void InvokeAsync<T1, T2, T3>(this AsyncEvent<Func<T1, T2, T3, Task>> eventHandler, T1 arg1, T2 arg2, T3 arg3)
        {
            var subscribers = eventHandler.Subscriptions;
            foreach (var subscription in subscribers)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await subscription.Invoke(arg1, arg2, arg3).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Something went wrong while executing subscription {subscription.Target}/{subscription.Method.Name} of {eventHandler.GetName()}.");
                        Console.WriteLine(ex);
                    }
                });
            }
        }
        public static void InvokeAsync<T1, T2, T3, T4>(this AsyncEvent<Func<T1, T2, T3, T4, Task>> eventHandler, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            var subscribers = eventHandler.Subscriptions;
            foreach (var subscription in subscribers)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await subscription.Invoke(arg1, arg2, arg3, arg4).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Something went wrong while executing subscription {subscription.Target}/{subscription.Method.Name} of {eventHandler.GetName()}.");
                        Console.WriteLine(ex);
                    }
                });
            }
        }
        public static void InvokeAsync<T1, T2, T3, T4, T5>(this AsyncEvent<Func<T1, T2, T3, T4, T5, Task>> eventHandler, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            var subscribers = eventHandler.Subscriptions;
            foreach (var subscription in subscribers)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await subscription.Invoke(arg1, arg2, arg3, arg4, arg5).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Something went wrong while executing subscription {subscription.Target}/{subscription.Method.Name} of {eventHandler.GetName()}.");
                        Console.WriteLine(ex);
                    }
                });
            }
        }
    }
}
