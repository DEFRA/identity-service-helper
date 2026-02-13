// <copyright file="CreateApplication.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Applications.Commands.Create;

public class CreateApplication : Application
{
    public Guid OperatorId { get; set; } = Guid.Empty;
}
