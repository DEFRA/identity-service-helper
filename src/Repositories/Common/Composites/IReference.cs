// <copyright file="IReference.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Common.Composites;

public interface IReference
{
    Task<bool> ValidateReferenceById(Guid id, CancellationToken cancellationToken = default);
}
