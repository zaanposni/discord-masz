namespace MASZ.Logger
{
    public class ConsoleLogger : ILogger
    {
        private string _categoryName;
        private readonly LogLevel _level = LogLevel.Information;
        private readonly string _MASZPrefix = "MASZ.";
        private readonly string _dNetPrefix = "Discord.";
        private readonly string _dNetClientPrefix = "Discord.WebSocket.DiscordSocketClient";

        public ConsoleLogger(string categoryName)
        {
            _categoryName = categoryName;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            string message = formatter(state, exception);

            string shortLogLevel;

            switch (logLevel)
            {
                case LogLevel.Trace:
                    shortLogLevel = "T";
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
                case LogLevel.Debug:
                    shortLogLevel = "D";
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
                case LogLevel.Information:
                    shortLogLevel = "I";
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Warning:
                    shortLogLevel = "W";
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Error:
                    shortLogLevel = "E";
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogLevel.Critical:
                    shortLogLevel = "C";
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                default:
                    shortLogLevel = "N";
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
            }

            if (_categoryName.StartsWith(_MASZPrefix))
            {
                _categoryName = _categoryName.Split('.').Last()
                                             .Replace("RequestLoggingMiddleware", "ReqLog")
                                             .Replace("Command", "Cmd")
                                             .Replace("Interface", "I");
            }
            else if (_categoryName.StartsWith(_dNetClientPrefix))
            {
                _categoryName = _categoryName.Replace(_dNetClientPrefix, "DNet.Client");
            }
            else if (_categoryName.StartsWith(_dNetPrefix))
            {
                _categoryName = _categoryName.Replace(_dNetPrefix, "DNET.");
            }

            string currentTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            string prefix = $"[{currentTime}] [{shortLogLevel}] {_categoryName}[{eventId.Id}]: ";

            Console.WriteLine($"{prefix}{message}");
            if (exception != null)
            {
                Console.WriteLine(exception.Message);
                if (exception.StackTrace != null)
                {
                    Console.WriteLine(exception.StackTrace);
                }
            }
            Console.ResetColor();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= _level;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}
