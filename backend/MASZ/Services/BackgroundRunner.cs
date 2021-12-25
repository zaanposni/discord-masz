namespace MASZ.Services
{
    public class BackgroundRunner
    {
        private readonly ILogger<BackgroundRunner> _logger;
        private readonly IServiceProvider _serviceProvider;
        public BackgroundRunner(ILogger<BackgroundRunner> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public void QueueAction<T1, T2>(Func<IServiceScope, T1, T2, Task> action, T1 param1, T2 param2)
        {
            _logger.LogInformation("Queueing new action for " + action.Target.ToString());
            Task task = new(async () =>
            {
                _logger.LogInformation("Starting action.");
                using var scope = _serviceProvider.CreateScope();
                await action.Invoke(scope, param1, param2);
                _logger.LogInformation("Finished action.");
            });
            task.Start();
        }
    }
}
