// <copyright file="GetApplications.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Applications.Queries;

public class GetApplications
{
    public string? Status { get; set; } = "Active";
}
