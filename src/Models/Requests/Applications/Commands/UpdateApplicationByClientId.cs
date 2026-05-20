// <copyright file="UpdateApplicationByClientId.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Applications.Commands;

using System.ComponentModel;
using Defra.Identity.Models.Requests.Applications.Commands.Base;
using Defra.Identity.Models.Requests.Common;

public class UpdateApplicationByClientId : ApplicationWriteOperationBase, IOperationById<Guid>
{
    [Description(OpenApiMetadata.Applications.Id)]
    public Guid Id { get; set; }

    public string GetLoggableId()
    {
        return Id.ToString() ?? throw new InvalidOperationException("Id has not been set");
    }
}
