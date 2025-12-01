// <copyright file="UserAccount.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgre.Database.Entities;

using Defra.Identity.Postgre.Database.Entities.Base;

public class Applications : BaseUpdateEntity
{
    public string Upn { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public bool AccountEnabled { get; set; }

    public ICollection<Federation> Federations { get; set; }

    public ICollection<Enrolment> Enrolments { get; set; }
}
