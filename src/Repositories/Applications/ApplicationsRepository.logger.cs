// <copyright file="ApplicationsRepository.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Applications;

using Microsoft.Extensions.Logging;

public partial class ApplicationsRepository
{
    [LoggerMessage(LogLevel.Information, "Getting single application")]
    partial void LogGettingSingleApplication();

    [LoggerMessage(LogLevel.Information, "Getting list of applications")]
    partial void LogGettingListOfApplications();

    [LoggerMessage(LogLevel.Information, "Creating application")]
    partial void LogCreatingApplication();

    [LoggerMessage(LogLevel.Information, "Updating application with id {Id}")]
    partial void LogUpdatingApplicationWithId(Guid id);

    [LoggerMessage(LogLevel.Information, "Deleting application with operator id {OperatorId}")]
    partial void LogDeletingApplicationWithOperatorId(Guid operatorId);

    [LoggerMessage(LogLevel.Warning, "Application not found for deletion")]
    partial void LogApplicationNotFoundForDeletion();
}
