using System;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace masz.Logger
{
    public class CustomLoggerProvider : ILoggerProvider
    {
        public void Dispose() { }

        public ILogger CreateLogger(string categoryName)
        {
            return new CustomConsoleLogger(categoryName);
        }

        public class CustomConsoleLogger : ILogger
        {
            private string _categoryName;
            private LogLevel _level = LogLevel.Information;
            private readonly string _maszPrefix = "masz.";
            private readonly string _dSharpPlusPrefix = "DSharpPlus.";

            public CustomConsoleLogger(string categoryName)
            {
                _categoryName = categoryName;
            }

            public void SetLogLevel(LogLevel logLevel)
            {
                _level = logLevel;
            }

            private bool IsBlocked(string message, LogLevel logLevel)
            {
                if (_categoryName == "DSharpPlus.BaseDiscordClient" || _categoryName == "D#.BaseDiscordClient")
                {
                    // this is really annoying... and not even a real warning...
                    if (message.Contains("Pre-emptive ratelimit triggered - waiting until") && logLevel == LogLevel.Warning)
                    {
                        return true;
                    }
                }
                return false;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                if (!IsEnabled(logLevel))
                {
                    return;
                }

                string message = formatter(state, exception);
                if (IsBlocked(message, logLevel)) { return; }

                string shortLogLevel = logLevel.ToString().ToUpper();
                switch(logLevel)
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

                if (_categoryName.StartsWith(_maszPrefix)) {
                    _categoryName = _categoryName.Split('.').Last()
                                                 .Replace("RequestLoggingMiddleware", "ReqLog")
                                                 .Replace("Command", "Cmd")
                                                 .Replace("Interface", "I");
                } else if (_categoryName.StartsWith(_dSharpPlusPrefix)) {
                    _categoryName = _categoryName.Replace("DSharpPlus.", "D#.");
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
}