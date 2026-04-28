// <copyright file="GetUserDelegatesById.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Permissions.Queries;

using System.ComponentModel;
using Defra.Identity.Models.Requests.Common;
using Defra.Identity.Models.Requests.Common.Queries;

public class GetUserDelegatesById : PagedQuery, IOperationById
{
    [Description(OpenApiMetadata.Users.Id)]
    public Guid Id { get; set; }
}
