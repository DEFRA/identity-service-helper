// <copyright file="UserAccount.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Livestock.Auth.Database.Entities;

using System.Data;
using Livestock.Auth.Database.Entities.Base;

public class UserAccount : BaseUpdateEntity
{
    public required string Upn { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public bool AccountEnabled { get; set; }

    public ICollection<Federation> Federations { get; set; } = new List<Federation>();

    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
}
