// <copyright file="ReferenceRulesBuilder.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>
namespace Defra.Identity.Services.Common.Builders.Rules;

using Defra.Identity.Repositories.Common.Composites;
using Defra.Identity.Repositories.Common.Exceptions;
using Defra.Identity.Services.Common.Builders.Rules.Models;
using Microsoft.Extensions.Logging;

public class ReferenceRulesBuilder<TService>
    where TService : class
{
    private List<ReferenceRule> ReferenceRules { get; } = [];

    public ReferenceRulesBuilder<TService> Add(IReference repository, Guid id, string description)
    {
        ReferenceRules.Add(new ReferenceRule(repository, id, description));

        return this;
    }

    public async Task Validate(string actionDescription, string primaryEntityDescription, CancellationToken cancellationToken, ILogger<TService> logger)
    {
        foreach (var rule in ReferenceRules)
        {
            var validAgainstReferenceRule = await rule.Repository.ValidateReferenceById(rule.Id, cancellationToken);

            if (!validAgainstReferenceRule)
            {
                logger.LogWarning(
                    "Execute {ActionDescription} {EntityDescription} failed reference rule '{Description}'",
                    actionDescription.ToLowerInvariant(),
                    primaryEntityDescription.ToLowerInvariant(),
                    rule.Description);

                throw new NotFoundException(rule.Description);
            }
        }
    }
}
