// <copyright file="Registration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Entities;

using Defra.Identity.Postgres.Database.Entities.Base;

public class Registration : BaseUpdateEntity
{
    public Guid CountryParishHoldingId { get; set; }

    public CountyParishHolding CountyParishHolding { get; set; } = null!;

    public Guid ApplicationId { get; set; }

    public Application Application { get; set; } = null!;

    public int StatusTypeId { get; set; }

    public StatusType Status { get; set; } = null!;

    public DateTime EnrolledAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public Guid CreatedBy { get; set; }

    public Guid? UpdatedBy { get; set; }
}
