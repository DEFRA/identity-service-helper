// <copyright file="CountyParishHolding.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

using Defra.Identity.Postgres.Database.Entities.Base;

namespace Defra.Identity.Postgres.Database.Entities;

public class CountyParishHolding : BaseUpdateEntity
{
    public string Id { get; set; }

    public int StatusTypeId { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime ProcessedAt { get; set; }

    public Guid UpdatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }
}
