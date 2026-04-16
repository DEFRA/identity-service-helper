// <copyright file="OperationById.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Common;

using System.ComponentModel;

public class OperationById : IOperationById
{
    [Description(OpenApiMetadata.Id)]
    public Guid Id { get; set; }
}
