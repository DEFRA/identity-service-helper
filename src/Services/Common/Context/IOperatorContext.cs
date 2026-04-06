// <copyright file="IOperatorContext.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Context;

public interface IOperatorContext
{
    public Guid OperatorId { get; }
}
