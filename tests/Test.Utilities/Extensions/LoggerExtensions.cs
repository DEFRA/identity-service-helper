// <copyright file="LoggerExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.Logging;

using System.Diagnostics.CodeAnalysis;
using NSubstitute;

[ExcludeFromCodeCoverage]
public static class LoggerExtensions
{
    public static void VerifyLogContainsOne(this ILogger logger, LogLevel logLevel, string message)
    {
        logger.Received(1).Log(
            logLevel,
            Arg.Any<EventId>(),
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Arg.Is<object>(v => v != null && v.ToString().Contains(message)),
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }
}
