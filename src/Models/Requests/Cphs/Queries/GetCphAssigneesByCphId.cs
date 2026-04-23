// <copyright file="GetCphAssigneesByCphId.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Cphs.Queries;

using System.ComponentModel;
using Defra.Identity.Models.Requests.Common;
using Defra.Identity.Models.Requests.Common.Queries;

public class GetCphAssigneesByCphId : PagedQuery, IOperationById
{
    [Description(OpenApiMetadata.Cphs.Id)]
    public Guid Id { get; set; }
}
