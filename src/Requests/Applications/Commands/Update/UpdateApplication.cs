// <copyright file="UpdateApplication.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Applications.Commands.Update;

public class UpdateApplication : Application
{
    public Guid Id { get; set; }

    public Guid OperatorId { get; set; } = Guid.Empty;
}
