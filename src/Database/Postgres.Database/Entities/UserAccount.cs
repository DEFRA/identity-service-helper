// <copyright file="UserAccount.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Entities;

using Defra.Identity.Postgres.Database.Entities.Base;

public class UserAccount : BaseUpdateEntity
{
    public string Upn { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public bool AccountEnabled { get; set; }

    public ICollection<Federation> Federations { get; set; }

    public ICollection<Delegation> Enrolments { get; set; }
}
