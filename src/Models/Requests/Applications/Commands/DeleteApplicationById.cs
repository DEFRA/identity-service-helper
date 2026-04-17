// <copyright file="DeleteApplicationById.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Applications.Commands;

using System.ComponentModel;

public class DeleteApplicationById
{
    [Description(OpenApiMetadata.Applications.Id)]
    public Guid Id { get; set; }
}
