using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace BeveragesApp.Helpers;

internal static class PageLogging
{
    private static ILoggerFactory _loggerFactory = NullLoggerFactory.Instance;

    public static void Initialize(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    public static ILogger<TPage> CreateLogger<TPage>() => _loggerFactory.CreateLogger<TPage>();

    /// <summary>
    /// Creates a logger with the specified category name.
    /// </summary>
    /// <param name="categoryName">The category name for the logger.</param>
    /// <returns>The logger instance.</returns>
   public static ILogger CreateLogger(string categoryName) => _loggerFactory.CreateLogger(categoryName);
}
