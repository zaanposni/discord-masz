using System;
using System.Threading.Tasks;
using masz.Events;

namespace masz.Services
{
    public interface IEventHandler
    {
        Task Invoke<T>(AsyncEventHandler<T> eventHandler, T eventArgs) where T : EventArgs;
    }
}
