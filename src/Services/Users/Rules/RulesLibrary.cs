// <copyright file="RulesLibrary.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Users.Rules;

using System.Diagnostics.CodeAnalysis;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Services.Common.Strategy.Rules.Models;

[ExcludeFromCodeCoverage]
public static class RulesLibrary
{
    public static class Existence
    {
        public static readonly EntityPredicate<UserAccounts> NotSoftDeleted =
            new(user => user.DeletedAt == null, "User must not be deleted");
    }
}
