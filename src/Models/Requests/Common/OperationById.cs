// <copyright file="OperationById.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Common;

using System.ComponentModel;

public abstract class OperationById : IOperationById
{
    [Description(OpenApiMetadata.Users.Id)]
    public Guid Id { get; set; }
}
