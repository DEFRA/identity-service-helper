// <copyright file="GetApplications.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Applications.Queries;

using System.ComponentModel;

public class GetApplications
{
    [Description(OpenApiMetadata.Applications.Status)]
    public string? Status { get; set; } = "Active";
}
