using System;
using System.Threading;
using System.Threading.Tasks;
using masz.Events;
using Microsoft.Extensions.Logging;

/*

This plugin is an example of how to create a background task.
Especially on some listeners like OnIdentityRegistered it is important to not block masz.
The website or a slash command may wait for a response so it is important to not block masz.
If you have a long running task you can use the Task.Run() method to run it in a background thread.

*/

namespace masz.Plugins
{
    public class ExampleBackgroundPlugin : BasePlugin, IBasePlugin
    {
        private readonly ILogger<ExampleBackgroundPlugin> _logger;
        public ExampleBackgroundPlugin() { }
        public ExampleBackgroundPlugin(ILogger<ExampleBackgroundPlugin> logger, IServiceProvider serviceProvider): base(serviceProvider)
        {
            _logger = logger;
        }

        public void RegisterEvents()
        {
            _identityManager.OnIdentityRegistered += OnIdentityRegistered;
        }

        private Task OnIdentityRegistered(IdentityRegisteredEventArgs e)
        {
            _logger.LogInformation($"Identity {e.GetIdentity().GetCurrentUser().Username} registered!");
            Task.Run(() => SomeAsyncBackgroundStuff());
            _logger.LogInformation("let masz continue...");
            return Task.CompletedTask;
        }

        private Task SomeAsyncBackgroundStuff()
        {
            Thread.Sleep(10000);
            _logger.LogInformation("end of non blocking stuff.");
            return Task.CompletedTask;
        }
    }
}