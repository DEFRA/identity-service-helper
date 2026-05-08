// <copyright file="UsersRepository.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Users;

using Microsoft.Extensions.Logging;

public partial class UsersRepository
{
    [LoggerMessage(LogLevel.Information, "Validating user account reference with id {Id}")]
    partial void LogValidatingUserAccountReferenceWithId(Guid id);

    [LoggerMessage(LogLevel.Information, "Getting all user accounts")]
    partial void LogGettingAllUserAccounts();

    [LoggerMessage(LogLevel.Information, "Getting single user account")]
    partial void LogGettingSingleUserAccount();

    [LoggerMessage(LogLevel.Information, "Getting list of user accounts")]
    partial void LogGettingListOfUserAccounts();

    [LoggerMessage(LogLevel.Information, "Creating user account with id {Id}")]
    partial void LogCreatingUserAccountWithId(Guid id);

    [LoggerMessage(LogLevel.Information, "Updating user account with id {Id}")]
    partial void LogUpdatingUserAccountWithId(Guid id);

    [LoggerMessage(LogLevel.Information, "Deleting user account with operator id {OperatorId}")]
    partial void LogDeletingUserAccountWithOperatorId(Guid operatorId);

    [LoggerMessage(LogLevel.Warning, "User account not found for deletion")]
    partial void LogUserAccountNotFoundForDeletion();
}
