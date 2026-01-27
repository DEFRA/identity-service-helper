// <copyright file="StatusType.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Entities;

using Defra.Identity.Postgres.Database.Entities.Base;

public class StatusType : BaseTypeEntity
{
    public ICollection<Application> Applications { get; set; } = new List<Application>();

    public ICollection<CountyParishHolding> CountyParishHoldings { get; set; } = new List<CountyParishHolding>();

    public ICollection<Delegation> Delegations { get; set; } = new List<Delegation>();

    public ICollection<Registration> Registrations { get; set; } = new List<Registration>();

    public ICollection<UserAccount> UserAccounts { get; set; } = new List<UserAccount>();
}
