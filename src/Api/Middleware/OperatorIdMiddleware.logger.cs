// <copyright file="OperatorIdMiddleware.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Middleware;

public partial class OperatorIdMiddleware
{
    [LoggerMessage(LogLevel.Error, "Error in {MiddlewareName}")]
    static partial void LogErrorInMiddleware(ILogger logger, string middlewareName, Exception exception);
}
