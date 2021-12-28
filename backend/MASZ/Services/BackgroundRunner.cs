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
        public void QueueAction<T1>(Func<IServiceScope, Task> action)
        {
            _logger.LogInformation("Queueing new action for " + action.Target.ToString());
            Task task = new(async () =>
            {
                _logger.LogDebug("Starting action.");
                using var scope = _serviceProvider.CreateScope();
                await action.Invoke(scope);
                _logger.LogDebug("Finished action.");
            });
            task.Start();
        }

        public void QueueAction<T1>(Func<IServiceScope, T1, Task> action, T1 param1)
        {
            _logger.LogInformation("Queueing new action for " + action.Target.ToString());
            Task task = new(async () =>
            {
                _logger.LogDebug("Starting action.");
                using var scope = _serviceProvider.CreateScope();
                await action.Invoke(scope, param1);
                _logger.LogDebug("Finished action.");
            });
            task.Start();
        }

        public void QueueAction<T1, T2>(Func<IServiceScope, T1, T2, Task> action, T1 param1, T2 param2)
        {
            _logger.LogInformation("Queueing new action for " + action.Target.ToString());
            Task task = new(async () =>
            {
                _logger.LogDebug("Starting action.");
                using var scope = _serviceProvider.CreateScope();
                await action.Invoke(scope, param1, param2);
                _logger.LogDebug("Finished action.");
            });
            task.Start();
        }

        public void QueueAction<T1, T2, T3>(Func<IServiceScope, T1, T2, T3, Task> action, T1 param1, T2 param2, T3 param3)
        {
            _logger.LogInformation("Queueing new action for " + action.Target.ToString());
            Task task = new(async () =>
            {
                _logger.LogDebug("Starting action.");
                using var scope = _serviceProvider.CreateScope();
                await action.Invoke(scope, param1, param2, param3);
                _logger.LogDebug("Finished action.");
            });
            task.Start();
        }

        public void QueueAction<T1, T2, T3, T4>(Func<IServiceScope, T1, T2, T3, T4, Task> action, T1 param1, T2 param2, T3 param3, T4 param4)
        {
            _logger.LogInformation("Queueing new action for " + action.Target.ToString());
            Task task = new(async () =>
            {
                _logger.LogDebug("Starting action.");
                using var scope = _serviceProvider.CreateScope();
                await action.Invoke(scope, param1, param2, param3, param4);
                _logger.LogDebug("Finished action.");
            });
            task.Start();
        }

        public void QueueAction<T1, T2, T3, T4, T5>(Func<IServiceScope, T1, T2, T3, T4, T5, Task> action, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            _logger.LogInformation("Queueing new action for " + action.Target.ToString());
            Task task = new(async () =>
            {
                _logger.LogDebug("Starting action.");
                using var scope = _serviceProvider.CreateScope();
                await action.Invoke(scope, param1, param2, param3, param4, param5);
                _logger.LogDebug("Finished action.");
            });
            task.Start();
        }

        public void QueueAction<T1, T2, T3, T4, T5, T6>(Func<IServiceScope, T1, T2, T3, T4, T5, T6, Task> action, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6)
        {
            _logger.LogInformation("Queueing new action for " + action.Target.ToString());
            Task task = new(async () =>
            {
                _logger.LogDebug("Starting action.");
                using var scope = _serviceProvider.CreateScope();
                await action.Invoke(scope, param1, param2, param3, param4, param5, param6);
                _logger.LogDebug("Finished action.");
            });
            task.Start();
        }
    }
}
