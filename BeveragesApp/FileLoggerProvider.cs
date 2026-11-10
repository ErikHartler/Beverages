using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace BeveragesApp;

internal sealed class FileLoggerProvider : ILoggerProvider
{
    private readonly string _logFilePath;
    private readonly ConcurrentDictionary<string, FileLogger> _loggers = new();
    private readonly object _writeLock = new();
    private bool _disposed;

    public FileLoggerProvider(string logFilePath)
    {
        _logFilePath = logFilePath;
        Directory.CreateDirectory(Path.GetDirectoryName(logFilePath)!);
    }

    public ILogger CreateLogger(string categoryName) =>
        _loggers.GetOrAdd(categoryName, name => new FileLogger(this, name));

    public void Dispose()
    {
        _disposed = true;
        _loggers.Clear();
    }

    private void Write(string category, LogLevel logLevel, EventId eventId, string message, Exception? exception)
    {
        if (_disposed)
        {
            return;
        }

        string exceptionText = exception is null ? string.Empty : Environment.NewLine + exception;
        string line = $"{DateTimeOffset.Now:O} [{logLevel}] [{category}] EventId={eventId.Id} {message}{exceptionText}{Environment.NewLine}";

        try
        {
            lock (_writeLock)
            {
                if (!_disposed)
                {
                    File.AppendAllText(_logFilePath, line);
                }
            }
        }
        catch (IOException)
        {
            // Logging failures must not interrupt app operations.
        }
        catch (UnauthorizedAccessException)
        {
            // Logging failures must not interrupt app operations.
        }
    }

    private sealed class FileLogger(FileLoggerProvider provider, string categoryName) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) =>
            logLevel != LogLevel.None && !provider._disposed;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            provider.Write(categoryName, logLevel, eventId, formatter(state, exception), exception);
        }
    }
}
