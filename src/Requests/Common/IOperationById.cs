// <copyright file="IOperationById.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Common;

public interface IOperationById
{
    public Guid Id { get; set; }
}
