// <copyright file="StrategyBuilderBase.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Builders.Strategy.Base;

using Defra.Identity.Services.Common.Builders.Strategy.Constants;
using Defra.Identity.Services.Common.Context;
using FluentValidation;
using Microsoft.Extensions.Logging;
using ValidationResult = FluentValidation.Results.ValidationResult;

public abstract class StrategyBuilderBase<TService, TBuilder>
    where TService : class
    where TBuilder : StrategyBuilderBase<TService, TBuilder>
{
    protected ILogger<TService>? Logger { get; private set; }

    protected CancellationToken? CancellationToken { get; private set; }

    protected IOperatorContext? OperatorContext { get; private set; }

    protected string? PrimaryEntityDescription { get; private set; }

    protected string? ActionDescription { get; private set; }

    private Action? SetupAction { get; set; }

    private Func<Task<ValidationResult>>? ValidateAction { get; set; }

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

    public TBuilder WithPrimaryEntityDescription(string primaryEntityDescription)
    {
        PrimaryEntityDescription = primaryEntityDescription;
        return (TBuilder)this;
    }

    public TBuilder WithActionDescription(string actionDescription)
    {
        ActionDescription = actionDescription;
        return (TBuilder)this;
    }

    public TBuilder WithSetup(Action setupAction)
    {
        SetupAction = setupAction;
        return (TBuilder)this;
    }

    public TBuilder WithRequestValidation(Func<Task<ValidationResult>> validateAction)
    {
        ValidateAction = validateAction;
        return (TBuilder)this;
    }

    protected void ExecuteSetup()
    {
        SetupAction?.Invoke();
    }

    protected async Task ExecuteRequestValidation()
    {
        if (Logger == null)
        {
            throw new InvalidOperationException(StrategyBuilderConstants.Errors.LoggerRequired);
        }

        if (PrimaryEntityDescription == null)
        {
            throw new InvalidOperationException(StrategyBuilderConstants.Errors.PrimaryEntityDescriptionRequired);
        }

        if (ActionDescription == null)
        {
            throw new InvalidOperationException(StrategyBuilderConstants.Errors.ActionDescriptionRequired);
        }

        if (ValidateAction != null)
        {
            var validationResult = await ValidateAction();

            if (!validationResult.IsValid)
            {
                Logger.LogWarning(
                    "Execute {ActionDescription} {EntityDescription} failed basic validation",
                    ActionDescription.ToLowerInvariant(),
                    PrimaryEntityDescription.ToLowerInvariant());

                throw new ValidationException(validationResult.Errors);
            }
        }
    }
}
