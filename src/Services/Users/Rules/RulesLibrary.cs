// <copyright file="RulesLibrary.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Users.Rules;

using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Services.Common.Builders.Predicates.Models;

public static class RulesLibrary
{
    public static class Existence
    {
        public static readonly EntityPredicate<UserAccounts> NotSoftDeleted =
            new(delegation => delegation.DeletedAt == null, "User account must not be deleted");
    }
}
