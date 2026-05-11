// <copyright file="IOperationById.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Common;

public interface IOperationById
{
    Guid Id { get; set; }
}
