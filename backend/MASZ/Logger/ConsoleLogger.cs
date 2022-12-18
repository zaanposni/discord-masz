using Spectre.Console;

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

            string shortLogLevel = logLevel.ToString().ToUpper();
            switch(logLevel)
            {
                case LogLevel.Trace:
                    shortLogLevel = "[grey]T[/]";
                    break;
                case LogLevel.Debug:
                    shortLogLevel = "[grey]D[/]";
                    break;
                case LogLevel.Information:
                    shortLogLevel = "[blue]I[/]";
                    break;
                case LogLevel.Warning:
                    shortLogLevel = "[yellow]W[/]";
                    break;
                case LogLevel.Error:
                    shortLogLevel = "[red]E[/]";
                    break;
                case LogLevel.Critical:
                    shortLogLevel = "[red]C[/]";
                    break;
                default:
                    shortLogLevel = "N";
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

            AnsiConsole.Write($"[{currentTime}] [");
            AnsiConsole.Markup(shortLogLevel);
            AnsiConsole.WriteLine("] " + _categoryName + "[" + eventId.Id + "]: " + message);

            if (exception != null)
            {
                AnsiConsole.WriteException(exception);
            }
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
