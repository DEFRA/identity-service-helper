// <copyright file="IReferenceRule.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Strategy.Rules.Models;

using Defra.Identity.Repositories.Common.Exceptions;
using Microsoft.Extensions.Logging;

public interface IReferenceRule<in TService>
    where TService : class
{
    Task Validate(
        string actionDescription,
        string primaryEntityDescription,
        ILogger<TService> logger,
        CancellationToken cancellationToken);
}
