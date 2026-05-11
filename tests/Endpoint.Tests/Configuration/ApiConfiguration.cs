// <copyright file="ApiConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Endpoint.Tests.Configuration;

public class ApiConfiguration
{
    public string ApiKey { get; set; } = null!;

    public string OperatorId { get; set; } = null!;

    public Guid CorrelationId { get; set; }

    public Guid DefraUserId { get; set; }

    public Guid OwnerUserId { get; set; }

    public Guid KeeperUserId { get; set; }
}
