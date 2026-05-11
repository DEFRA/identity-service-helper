// <copyright file="UpdateApplicationById.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Applications.Commands;

using System.ComponentModel;

public class UpdateApplicationById
{
    [Description(OpenApiMetadata.Applications.Id)]
    public Guid Id { get; set; }
}
