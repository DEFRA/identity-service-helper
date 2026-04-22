// <copyright file="GetCphDelegatesByDelegatorId.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Users.Queries;

using System.ComponentModel;
using Defra.Identity.Models.Requests.Common;
using Defra.Identity.Models.Requests.Common.Queries;

public class GetCphDelegatesByDelegatorId : PagedQuery, IOperationById
{
    [Description(OpenApiMetadata.Users.Id)]
    public Guid Id { get; set; }
}
