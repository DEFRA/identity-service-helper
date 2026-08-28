// <copyright file="IUserAccountsProvider.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Providers;

using Defra.Identity.KeeperReferenceData.Models.UserAccounts;

public interface IUserAccountsProvider : IDisposable
{
    /// <summary>
    /// Ensures the user account exists in KRDS, refreshing the profile from the supplied claims and
    /// rebuilding the CPH association snapshot from the SAM mastered party data.
    /// </summary>
    /// <param name="request">The identity provider claims.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The ensured account and whether KRDS created it.</returns>
    Task<EnsureUserAccountResult> EnsureUserAccount(EnsureUserAccountRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a user account by identity provider subject without triggering a refresh.
    /// </summary>
    /// <param name="subject">The identity provider subject claim.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The account, or null when the subject is not recognised.</returns>
    Task<UserAccount?> GetUserAccountBySubject(string subject, CancellationToken cancellationToken);
}
