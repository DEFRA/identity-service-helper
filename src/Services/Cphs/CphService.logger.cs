// <copyright file="CphService.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Cphs;

using Microsoft.Extensions.Logging;

public partial class CphService
{
    [LoggerMessage(LogLevel.Information, "Getting county parish holding by id {Id}")]
    partial void LogGettingCountyParishHoldingById(Guid id);

    [LoggerMessage(LogLevel.Warning, "County parish holding with id {Id} not found")]
    partial void LogCountyParishHoldingWithIdNotFound(Guid id);

    [LoggerMessage(LogLevel.Information, "Getting all county parish holdings by page")]
    partial void LogGettingAllCountyParishHoldingsByPage();

    [LoggerMessage(LogLevel.Information, "Expiring county parish holding with id {Id} by operator {OperatorId}")]
    partial void LogExpiringCountyParishHoldingWithIdByOperatorid(Guid id, Guid operatorId);

    [LoggerMessage(LogLevel.Warning, "County parish holding with id {Id} is already expired")]
    partial void LogCountyParishHoldingWithIdIsAlreadyExpired(Guid id);

    [LoggerMessage(LogLevel.Information, "Deleting county parish holding with id {Id} by operator {OperatorId}")]
    partial void LogDeletingCountyParishHoldingWithIdByOperatorid(Guid id, Guid operatorId);
}
