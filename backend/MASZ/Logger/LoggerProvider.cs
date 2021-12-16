namespace MASZ.Logger
{
    public class LoggerProvider : ILoggerProvider
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new ConsoleLogger(categoryName);
        }
    }
}