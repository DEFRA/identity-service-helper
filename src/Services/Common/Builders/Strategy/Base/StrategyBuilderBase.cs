// <copyright file="StrategyBuilderBase.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Builders.Strategy.Base;

using Defra.Identity.Services.Common.Context;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

public abstract class StrategyBuilderBase<TService, TBuilder>
    where TService : class
    where TBuilder : StrategyBuilderBase<TService, TBuilder>
{
    protected ILogger<TService>? Logger { get; private set; }

    protected CancellationToken? CancellationToken { get; private set; }

    protected IOperatorContext? OperatorContext { get; private set; }

    protected string? EntityDescription { get; private set; }

    protected string? ActionDescription { get; private set; }

    protected Func<Task<ValidationResult>>? ValidateAction { get; private set; }

    public TBuilder WithLogger(ILogger<TService> logger)
    {
        Logger = logger;
        return (TBuilder)this;
    }

    public TBuilder WithCancellationToken(CancellationToken cancellationToken)
    {
        CancellationToken = cancellationToken;
        return (TBuilder)this;
    }

    public TBuilder WithOperatorContext(IOperatorContext operatorContext)
    {
        OperatorContext = operatorContext;
        return (TBuilder)this;
    }

    public TBuilder WithEntityDescription(string entityDescription)
    {
        EntityDescription = entityDescription;
        return (TBuilder)this;
    }

    public TBuilder WithActionDescription(string actionDescription)
    {
        ActionDescription = actionDescription;
        return (TBuilder)this;
    }

    public TBuilder WithRequestValidation(Func<Task<ValidationResult>> validateAction)
    {
        ValidateAction = validateAction;
        return (TBuilder)this;
    }
}
