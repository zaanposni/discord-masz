using System;
using System.Threading.Tasks;
using masz.Events;
using Microsoft.Extensions.Logging;

namespace masz.Services
{
    public class EventHandler: IEventHandler
    {
        private readonly ILogger<EventHandler> _logger;
        public EventHandler(ILogger<EventHandler> logger)
        {
            _logger = logger;
        }
        public async Task Invoke<T>(AsyncEventHandler<T> eventHandler, T eventArgs) where T : EventArgs
        {
            try {
                if (eventHandler != null) {
                    await eventHandler(eventArgs);
                }
            } catch (Exception e) {
                _logger.LogError(e, $"Error while handling event '{eventHandler?.Method?.Name}'.");
            }
        }
    }
}