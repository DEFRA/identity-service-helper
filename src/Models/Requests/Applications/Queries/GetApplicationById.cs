// <copyright file="GetApplicationById.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Applications.Queries;

using System.ComponentModel;

public class GetApplicationById
{
    [Description(OpenApiMetadata.Applications.Id)]
    public Guid Id { get; set; }
}
