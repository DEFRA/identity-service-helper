// <copyright file="CreateApplication.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Applications.Commands;

using System.ComponentModel;
using Defra.Identity.Models.Requests.Applications.Commands.Base;

public class CreateApplication : ApplicationWriteOperationBase
{
    [Description(OpenApiMetadata.Applications.Id)]
    public Guid Id { get; set; }
}
