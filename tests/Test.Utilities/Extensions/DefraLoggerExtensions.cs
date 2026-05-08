// <copyright file="DefraLoggerExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.Logging;

using System.Diagnostics.CodeAnalysis;
using NSubstitute;

[ExcludeFromCodeCoverage]
public static class DefraLoggerExtensions
{
    public static ILogger<T> CreateNSubstituteLogger<T>()
        where T : class
    {
        var logger = Substitute.For<ILogger<T>>();
        logger.IsEnabled(Arg.Any<LogLevel>()).Returns(true);
        return logger;
    }

    public static void VerifyLogExceptionTypeOne<TException>(this ILogger logger, LogLevel logLevel)
        where TException : Exception
    {
        var matchingCalls = logger.ReceivedCalls()
            .Where(call => call.GetMethodInfo().Name == nameof(ILogger.Log))
            .Where(call => (LogLevel)call.GetArguments()[0] == logLevel)
            .Where(call => call.GetArguments()[3] is TException)
            .ToList();

        Assert.Single(matchingCalls);
    }

    public static void VerifyLogExceptionTypeOne<TException>(this ILogger logger, LogLevel logLevel, string message)
        where TException : Exception
    {
        var matchingCalls = logger.ReceivedCalls()
            .Where(call => call.GetMethodInfo().Name == nameof(ILogger.Log))
            .Where(call => (LogLevel)call.GetArguments()[0] == logLevel)
            .Where(call => (call.GetArguments()[2]?.ToString() ?? string.Empty).Contains(message, StringComparison.Ordinal))
            .Where(call => call.GetArguments()[3] is TException)
            .ToList();

        Assert.Single(matchingCalls);
    }

    public static void VerifyLogContainsOne(this ILogger logger, LogLevel logLevel, string message)
    {
        VerifyLogContains(logger, logLevel, message, 1);
    }

    public static void VerifyLogContains(this ILogger logger, LogLevel logLevel, string message, int expectedCount)
    {
        var matchingCalls = logger.ReceivedCalls()
            .Where(IsLoggerLogCall)
            .Where(call => (LogLevel)call.GetArguments()[0] == logLevel)
            .Where(call => (call.GetArguments()[2]?.ToString() ?? string.Empty).Contains(message, StringComparison.Ordinal))
            .ToList();

        matchingCalls.Count.ShouldBe(expectedCount);
    }

    public static void VerifyLogTemplateOne(this ILogger logger, LogLevel logLevel, string originalFormat)
    {
        var matchingCalls = logger.ReceivedCalls()
            .Where(IsLoggerLogCall)
            .Where(call => (LogLevel)call.GetArguments()[0] == logLevel)
            .Where(call => HasOriginalFormat(call, originalFormat))
            .ToList();

        matchingCalls.Count.ShouldBe(1);
    }

    private static bool IsLoggerLogCall(NSubstitute.Core.ICall call)
    {
        var method = call.GetMethodInfo();
        return method.Name == nameof(ILogger.Log) && method.DeclaringType == typeof(ILogger);
    }

    private static bool HasOriginalFormat(NSubstitute.Core.ICall call, string originalFormat)
    {
        return call.GetArguments()[2] is IEnumerable<KeyValuePair<string, object?>> state &&
               state.Any(x =>
                   x.Key == "{OriginalFormat}" &&
                   Equals(x.Value, originalFormat));
    }
}
