using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace VGStandard.Core.Utility.Logger;

public static class FileLoggerExtentions
{
    public static string LogDelimeter = " ";
    public static string MessageDelimeter = "<->";
    public static string _formatString = "[{Client-IpAddress-ProcessId}] [{Operation}] [{ErrorMessage}]";
    public static ILoggingBuilder AddFileLogger(this ILoggingBuilder builder, Action<FileLoggerOptions> configure)
    {
        builder.Services.AddSingleton<ILoggerProvider, FileLoggerProvider>();
        builder.Services.Configure(configure);
        return builder;
    }

    private static Action<ILogger,string,string,string, Exception> _logErrorWithCommonLogggingFormat;
    private static Action<ILogger, string, string, string, Exception> _logDebugWithCommonLogggingFormat;
    private static Action<ILogger, string, string, string, Exception> _logCriticalWithCommonLogggingFormat;
    private static Action<ILogger, string, string, string, Exception> _logInformationWithCommonLogggingFormat;


    static FileLoggerExtentions()
    {
        _logErrorWithCommonLogggingFormat = LoggerMessage.Define<string,string,string>(
            logLevel: LogLevel.Error,
            eventId: 1,
            formatString: _formatString);

        _logDebugWithCommonLogggingFormat = LoggerMessage.Define<string, string, string>(
            logLevel: LogLevel.Debug,
            eventId: 1,
            formatString: _formatString);

        _logCriticalWithCommonLogggingFormat = LoggerMessage.Define<string, string, string>(
            logLevel: LogLevel.Critical,
            eventId: 1,
            formatString: _formatString);

        _logInformationWithCommonLogggingFormat = LoggerMessage.Define<string, string, string>(
            logLevel: LogLevel.Information,
            eventId: 1,
            formatString: _formatString);
    }

    public static void LogWithCommonLogggingFormat(
        this ILogger logger, LogLevel logLevel, string client, string Operation,string Message,Exception ex)
    {
        Process currentProcess = Process.GetCurrentProcess();
        var clientIpAddressProcessId = client + "-" + UtilityMethods.GetMachineIPAddress() + "-" + currentProcess.Id.ToString();

        switch (logLevel)
        {
            case LogLevel.Error:
                _logErrorWithCommonLogggingFormat(logger, clientIpAddressProcessId, Operation, GetLogMessage(Message,ex), ex);
                break;

            case LogLevel.Critical:
                _logCriticalWithCommonLogggingFormat(logger, clientIpAddressProcessId, Operation, GetLogMessage(Message, ex), ex);
                break;

            case LogLevel.Debug:
                _logDebugWithCommonLogggingFormat(logger, clientIpAddressProcessId, Operation, GetLogMessage(Message, ex), ex);
                break;

            case LogLevel.Information:
                _logInformationWithCommonLogggingFormat(logger, clientIpAddressProcessId, Operation, GetLogMessage(Message, ex), ex);
                break;

            default:
                _logDebugWithCommonLogggingFormat(logger, clientIpAddressProcessId, Operation, Message + GetLogMessage(Message, ex), ex);
                break;
        }
    }

    private static string GetLogMessage(string message,Exception ex)
    {
        return ex != null ? message + MessageDelimeter + ex?.GetFullMessage() : message;
    }
}
