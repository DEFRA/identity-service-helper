// <copyright file="SutProvider.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Tests;

using System.Diagnostics.CodeAnalysis;
using Defra.Identity.Services.Common.Context;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

#pragma warning disable SA1201 // Elements should appear in the correct order // Ordering is important for readability

[ExcludeFromCodeCoverage]
public class SutProvider<TService>
    where TService : class
{
    private readonly Func<IOperatorContext?, TService> serviceFactory;

    private readonly IOperatorContext? defaultOperatorContext;

    private SutProvider(Func<IOperatorContext?, TService> serviceFactory, IOperatorContext? operatorContext = null)
    {
        this.serviceFactory = serviceFactory;
        this.defaultOperatorContext = operatorContext;
    }

    public static SutProvider<TService> CreateFor(
        Func<IOperatorContext?, TService> serviceFactory,
        IOperatorContext? operatorContext = null)
    {
        return new SutProvider<TService>(serviceFactory, operatorContext);
    }

    public TService WithOperatorId
    {
        get
        {
            if (defaultOperatorContext == null)
            {
                throw new InvalidOperationException("Default operator context not set");
            }

            defaultOperatorContext!.OperatorId.Returns(Guid.NewGuid());

            return this.serviceFactory(defaultOperatorContext);
        }
    }

    public TService WithoutOperatorId
    {
        get
        {
            if (defaultOperatorContext == null)
            {
                throw new InvalidOperationException("Default operator context not set");
            }

            defaultOperatorContext!.OperatorId.Throws(new Exception("Simulated operator id retrieval failure"));

            return this.serviceFactory(defaultOperatorContext);
        }
    }

    public TService WithoutOperatorContext => this.serviceFactory(null);

    public TService WithOperatorIdOf(Guid operatorId)
    {
        if (defaultOperatorContext == null)
        {
            throw new InvalidOperationException("Default operator context not set");
        }

        defaultOperatorContext!.OperatorId.Returns(operatorId);

        return this.serviceFactory(defaultOperatorContext);
    }
}
