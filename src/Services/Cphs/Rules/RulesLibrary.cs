// <copyright file="RulesLibrary.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Cphs.Rules;

using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Services.Common.Builders.Predicates.Models;

public static class RulesLibrary
{
    public static class Conflict
    {
        public static readonly EntityPredicate<CountyParishHoldings> NotAlreadyExpired =
            new(delegation => delegation.ExpiredAt == null, "County parish holding must not have already expired");
    }
}
