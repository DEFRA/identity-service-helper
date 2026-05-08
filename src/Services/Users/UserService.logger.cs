// <copyright file="UserService.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Users;

using Microsoft.Extensions.Logging;

public partial class UserService
{
    [LoggerMessage(LogLevel.Information, "Getting all users, includeHidden: {IncludeHidden}")]
    partial void LogGettingAllUsersIncludeHidden(string includeHidden);

    [LoggerMessage(LogLevel.Information, "Getting user by id {Id}")]
    partial void LogGettingUserById(Guid id);

    [LoggerMessage(LogLevel.Warning, "User with id {Id} not found")]
    partial void LogUserWithIdNotFound(Guid id);

    [LoggerMessage(LogLevel.Information, "Upserting user with id {Id}")]
    partial void LogUpsertingUserWithId(Guid id);

    [LoggerMessage(LogLevel.Information, "User with id {Id} found, updating")]
    partial void LogUserWithIdFoundUpdating(Guid id);

    [LoggerMessage(LogLevel.Information, "User with id {Id} not found, creating")]
    partial void LogUserWithIdNotFoundCreating(Guid id);

    [LoggerMessage(LogLevel.Information, "Updating user with id {Id}")]
    partial void LogUpdatingUserWithId(Guid id);

    [LoggerMessage(LogLevel.Information, "Creating new user with email {Email}")]
    partial void LogCreatingNewUserWithEmail(string email);

    [LoggerMessage(LogLevel.Information, "Deleting user with id {Id} by operator {OperatorId}")]
    partial void LogDeletingUserWithIdByOperatorId(Guid id, Guid operatorId);
}
