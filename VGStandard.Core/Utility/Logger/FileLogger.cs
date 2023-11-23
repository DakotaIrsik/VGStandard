using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace VGStandard.Core.Utility.Logger;

public class FileLogger : ILogger
{
    protected readonly FileLoggerProvider _fileLoggerProvider;

    public FileLogger([NotNull] FileLoggerProvider fileLoggerProvider)
    {
        _fileLoggerProvider = fileLoggerProvider;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel != LogLevel.None;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var threadId = Thread.CurrentThread.ManagedThreadId; // Get the current thread ID to use in the log file.

        Task.Run(async () =>
        {
            if (!string.IsNullOrEmpty(_fileLoggerProvider.Options.FolderPath) && !string.IsNullOrEmpty(_fileLoggerProvider.Options.FilePath))
            {
                // Run in a seperate task so the thread isn't waiting for it to be finished.
                var fullFilePath = _fileLoggerProvider.Options.FolderPath + "/" + _fileLoggerProvider.Options.FilePath.Replace("{date}", DateTimeOffset.UtcNow.ToString("yyyyMMdd")); // Get the full log file path. Seperated by day.
                var logRecord = string.Format("{0} [{1}] [{2}] {3} [{4}]", "[" + DateTimeOffset.UtcNow.ToString("yyyy-MM-dd\\THH:mm:ss.fff\\Z") + "]", logLevel.ToString(), "ZE-Manager", formatter(state, exception), exception != null ? exception.StackTrace : ""); // Format the log entry.


                // Ensure that only one thread can write to the text file to avoid issues with opening the text file.
                await _fileLoggerProvider.WriteFileLock.WaitAsync();

                // Write the log entry to the text file.
                using (var streamWriter = new StreamWriter(fullFilePath, true))
                {
                    await streamWriter.WriteLineAsync(logRecord);
                }

                // Ensure that the lock is released once the log entry has been written to the text file.
                _fileLoggerProvider.WriteFileLock.Release();
            }


        });
    }
}
