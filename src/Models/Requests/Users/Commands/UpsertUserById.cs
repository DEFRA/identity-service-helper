// <copyright file="UpsertUserById.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Users.Commands;

using System.ComponentModel;
using Defra.Identity.Models.Requests.Common;
using Defra.Identity.Models.Requests.Users.Commands.Base;

public class UpsertUserById : UserWriteOperationBase, IOperationById<Guid?>
{
    [Description(OpenApiMetadata.Users.Id)]
    public Guid? Id { get; set; } = null;

    public string GetLoggableId()
    {
        return Id?.ToString() ?? throw new InvalidOperationException("Id has not been set");
    }
}
