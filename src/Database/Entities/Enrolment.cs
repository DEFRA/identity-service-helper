// <copyright file="Enrolment.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Livestock.Auth.Database.Entities;

using Livestock.Auth.Database.Entities.Base;

public class Enrolment : BaseUpdateEntity
{
    public required Guid UserAccountId { get; init; }

    public UserAccount UserAccount { get; set; }

    public Guid ApplicationId { get; set; }

    public required string CphId { get; set; }

    public required string Role { get; set; }

    public string Scope { get; set; }

    public string Status { get; set; }

    public DateTime EnrolledAt { get; set; }

    public DateTime ExpiresAt { get; set; }
}
