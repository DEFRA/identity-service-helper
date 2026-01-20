// <copyright file="Registration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Entities;

using Defra.Identity.Postgres.Database.Entities.Base;

public class Registration : BaseUpdateEntity
{
    public Guid CreatedBy { get; set; }

    public Guid UpdatedBy { get; set; }
}
