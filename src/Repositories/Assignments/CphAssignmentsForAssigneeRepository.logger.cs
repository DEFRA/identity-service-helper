// <copyright file="CphAssignmentsForAssigneeRepository.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Assignments;

using Microsoft.Extensions.Logging;

public partial class CphAssignmentsForAssigneeRepository
{
    [LoggerMessage(LogLevel.Information, "Getting list of assignments for user account")]
    partial void LogGettingListOfAssignmentsForUserAccount();
}
