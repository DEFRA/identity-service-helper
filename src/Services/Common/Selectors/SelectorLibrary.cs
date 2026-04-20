// <copyright file="SelectorLibrary.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Selectors;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database.Entities;

public static class SelectorLibrary
{
    public static Expression<Func<UserAccounts, string>> UserDisplayNameSelector = user => user.DisplayName;
}
