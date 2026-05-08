// <copyright file="ApplicationService.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Applications;

using Microsoft.Extensions.Logging;

public partial class ApplicationService
{
    [LoggerMessage(LogLevel.Information, "Getting all applications")]
    partial void LogGettingAllApplications();

    [LoggerMessage(LogLevel.Information, "Getting application by id {Id}")]
    partial void LogGettingApplicationById(Guid id);

    [LoggerMessage(LogLevel.Warning, "Application with id {Id} not found")]
    partial void LogApplicationWithIdNotFound(Guid id);

    [LoggerMessage(LogLevel.Information, "Updating application with id {Id}")]
    partial void LogUpdatingApplicationWithId(Guid id);

    [LoggerMessage(LogLevel.Warning, "Application with id {Id} not found for update")]
    partial void LogApplicationWithIdNotFoundForUpdate(Guid id);

    [LoggerMessage(LogLevel.Information, "Creating new application with name {Name}")]
    partial void LogCreatingNewApplicationWithName(string name);

    [LoggerMessage(LogLevel.Information, "Deleting application with id {Id} by operator {OperatorId}")]
    partial void LogDeletingApplicationWithIdByOperator(Guid id, Guid operatorId);
}
