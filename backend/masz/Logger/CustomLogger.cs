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
            private readonly string _maszPrefix = "masz.";

            public CustomConsoleLogger(string categoryName)
            {
                _categoryName = categoryName;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                if (!IsEnabled(logLevel))
                {
                    return;
                }

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
                    _categoryName = _categoryName.Split('.').Last();
                }

                string prefix = $"[{shortLogLevel}] ";
                if (_categoryName == "RequestLoggingMiddleware")
                {
                    prefix += $"ReqLog[{eventId.Id}]: ";
                } else
                {
                    prefix += $"{_categoryName}[{eventId.Id}]: ";
                }

                Console.WriteLine($"{prefix}{formatter(state, exception)}");
                if (exception != null)
                {
                    Console.Write(exception.Message);
                    if (exception.StackTrace != null)
                    {
                        Console.Write(exception.StackTrace);
                    }
                }
                Console.ResetColor();
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                return null;
            }
        }
    }
}