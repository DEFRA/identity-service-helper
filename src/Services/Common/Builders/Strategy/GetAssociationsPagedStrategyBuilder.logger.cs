// <copyright file="GetAssociationsPagedStrategyBuilder.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Builders.Strategy;

using System.Linq.Expressions;
using Defra.Identity.Models.Requests.Common;
using Defra.Identity.Models.Requests.Common.Queries;
using Defra.Identity.Models.Responses.Common;
using Defra.Identity.Repositories.Common;
using Defra.Identity.Repositories.Common.Composites;
using Defra.Identity.Repositories.Common.Composites.Associations;
using Defra.Identity.Repositories.Common.Exceptions;
using Defra.Identity.Services.Common.Builders.Rules;
using Defra.Identity.Services.Common.Builders.Strategy.Base;
using Defra.Identity.Services.Common.Builders.Strategy.Constants;
using Microsoft.Extensions.Logging;

public partial class GetAssociationsPagedStrategyBuilder<TService, TPrimary, TAssociation>
{
    [LoggerMessage(LogLevel.Information, "Executing {ActionDescription} {EntityDescription} with id {Id}")]
    static partial void LogExecutingActionEntityWithId(
        ILogger<TService> logger,
        string actionDescription,
        string entityDescription,
        Guid id);

    [LoggerMessage(LogLevel.Warning, "{EntityDescription} with id {Id} not found")]
    static partial void LogEntityWithIdNotFound(
        ILogger<TService> logger,
        string entityDescription,
        Guid id);

    [LoggerMessage(LogLevel.Information, "Successfully executed {ActionDescription} {EntityDescription} with id {Id}")]
    static partial void LogSuccessfullyExecutedActionEntityWithId(
        ILogger<TService> logger,
        string actionDescription,
        string entityDescription,
        Guid id);
}
