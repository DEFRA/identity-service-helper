// <copyright file="UserAccountsProvider.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Providers;

using Microsoft.Extensions.Logging;

public partial class UserAccountsProvider
{
    [LoggerMessage(LogLevel.Information, "Ensuring user account")]
    partial void LogEnsuringUserAccount();

    [LoggerMessage(LogLevel.Information, "Getting user account by subject")]
    partial void LogGettingUserAccountBySubject();

    [LoggerMessage(LogLevel.Error, "Error deserializing user account body {ResponseBody}")]
    partial void LogErrorDeserializingUserAccountBodyResponsebody(string responseBody, Exception exception);
}
