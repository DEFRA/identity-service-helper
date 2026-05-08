// <copyright file="ExistenceRulesBuilder.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Builders.Rules;

using Defra.Identity.Models.Requests.Common;
using Defra.Identity.Repositories.Common.Exceptions;
using Defra.Identity.Services.Common.Builders.Predicates;
using Defra.Identity.Services.Common.Builders.Predicates.Models;
using Microsoft.Extensions.Logging;

public partial class ExistenceRulesBuilder<TService, TEntity>
{
    [LoggerMessage(LogLevel.Warning, "{EntityDescription} with id {Id} not found")]
    static partial void LogEntityDescriptionWithIdIdNotFound(ILogger<TService> logger, string entityDescription, Guid id);
}
