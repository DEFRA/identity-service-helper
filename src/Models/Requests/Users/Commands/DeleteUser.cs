// <copyright file="DeleteUser.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Users.Commands;

using System.ComponentModel;
using Defra.Identity.Models.Requests.Common;

public class DeleteUser : OperationById
{
    [Description(OpenApiMetadata.Users.Id)]
    public Guid OperatorId { get; set; } = Guid.Empty;
}
