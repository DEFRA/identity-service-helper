// <copyright file="EnsureUserAccountResult.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Models.UserAccounts;

/// <summary>
/// The outcome of an ensure call, where <paramref name="Created"/> indicates that KRDS created the account.
/// </summary>
/// <param name="Account">The ensured user account.</param>
/// <param name="Created">True when the account was created by this call.</param>
public record EnsureUserAccountResult(UserAccount Account, bool Created);
