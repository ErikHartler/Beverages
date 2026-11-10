using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace BeveragesApp;

internal static class PageLogging
{
    private static ILoggerFactory _loggerFactory = NullLoggerFactory.Instance;

    public static void Initialize(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    public static ILogger<TPage> CreateLogger<TPage>() => _loggerFactory.CreateLogger<TPage>();
}
