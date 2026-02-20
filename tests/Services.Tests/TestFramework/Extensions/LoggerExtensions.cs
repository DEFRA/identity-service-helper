// <copyright file="LoggerExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.Logging;

using Microsoft.Extensions.Logging;
using NSubstitute;

public static class LoggerExtensions
{
    public static void VerifyLogReceivedOnce(this ILogger logger, LogLevel logLevel, string message)
    {
        logger.Received(1).Log(
            logLevel,
            Arg.Any<EventId>(),
            Arg.Is<object>(v => v != null ? v.ToString().Contains(message) : false),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }
}
