// <copyright file="GetCphs.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Cphs.Queries;

using System.ComponentModel;
using Defra.Identity.Models.Requests.Common.Queries;

public class GetCphs : PagedQuery
{
    [Description(OpenApiMetadata.Expired)]
    public string? Expired { get; set; } = null;
}
