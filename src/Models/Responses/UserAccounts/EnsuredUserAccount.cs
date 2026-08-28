// <copyright file="EnsuredUserAccount.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Responses.UserAccounts;

/// <summary>
/// The outcome of an ensure call.
/// </summary>
/// <param name="Account">The refreshed account.</param>
/// <param name="Created">True when the account was created by this call.</param>
public record EnsuredUserAccount(UserAccount Account, bool Created);
