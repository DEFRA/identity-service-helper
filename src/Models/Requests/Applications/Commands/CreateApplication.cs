// <copyright file="CreateApplication.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Applications.Commands;

using System.ComponentModel;
using Defra.Identity.Models.Responses.Applications;

public class CreateApplication : Application
{
    [Description(OpenApiMetadata.Users.Id)]
    public Guid OperatorId { get; set; } = Guid.Empty;
}
