// <copyright file="ExternalMessagingRepository.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Messaging;

using Microsoft.Extensions.Logging;

public partial class ExternalMessagingRepository
{
    [LoggerMessage(LogLevel.Information, "Getting single external messaging record")]
    partial void LogGettingSingleExternalMessagingRecord();

    [LoggerMessage(LogLevel.Information, "Getting list of external messaging records")]
    partial void LogGettingListOfExternalMessagingRecords();

    [LoggerMessage(LogLevel.Information, "Creating external messaging record with id {Id}")]
    partial void LogCreatingExternalMessagingRecordWithId(int id);

    [LoggerMessage(LogLevel.Information, "Updating external messaging record with id {Id}")]
    partial void LogUpdatingExternalMessagingRecordWithId(int id);

    [LoggerMessage(LogLevel.Information, "Deleting external messaging record with operator id {OperatorId}")]
    partial void LogDeletingExternalMessagingRecordWithOperatorId(Guid operatorId);

    [LoggerMessage(LogLevel.Warning, "External messaging record not found for deletion")]
    partial void LogExternalMessagingRecordNotFoundForDeletion();
}
