// <copyright file="UserRepository.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Users;

using Microsoft.Extensions.Logging;

public partial class UserRepository
{
    [LoggerMessage(LogLevel.Information, "Getting single user account")]
    partial void LogGettingSingleUserAccount();

    [LoggerMessage(LogLevel.Information, "Getting list of user accounts")]
    partial void LogGettingListOfUserAccounts();

    [LoggerMessage(LogLevel.Information, "Creating user account")]
    partial void LogCreatingUserAccount();

    [LoggerMessage(LogLevel.Information, "Updating user account with id {Id}")]
    partial void LogUpdatingUserAccountWithId(Guid id);
}
