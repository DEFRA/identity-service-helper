// <copyright file="RulesLibrary.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Cphs.Rules;

using System.Diagnostics.CodeAnalysis;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Services.Common.Strategy.Rules.Models;

[ExcludeFromCodeCoverage]
public static class RulesLibrary
{
    public static class Existence
    {
        public static readonly EntityPredicate<CountyParishHoldings> NotSoftDeleted =
            new(user => user.DeletedAt == null, "County parish holding must not be deleted");
    }

    public static class Conflict
    {
        public static readonly EntityPredicate<CountyParishHoldings> NotAlreadyExpired =
            new(delegation => delegation.ExpiredAt == null, "County parish holding must not have already expired");
    }
}
