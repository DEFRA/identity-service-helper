// <copyright file="CphAssignmentsRepository.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Assignments;

using Microsoft.Extensions.Logging;

public partial class CphAssignmentsRepository
{
    [LoggerMessage(LogLevel.Information, "Getting list of county parish holding assignments")]
    partial void LogGettingListOfCountyParishHoldingAssignments();
}
