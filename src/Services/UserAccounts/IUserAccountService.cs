// <copyright file="IUserAccountService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.UserAccounts;

using Defra.Identity.Models.Requests.UserAccounts.Commands;
using Defra.Identity.Models.Responses.UserAccounts;

public interface IUserAccountService
{
    /// <summary>
    /// Ensures the keeper records data service holds an account for the supplied claims and returns
    /// the refreshed account, including its rebuilt County Parish Holding associations.
    /// </summary>
    /// <param name="request">The identity provider claims captured at logon.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The refreshed account and whether it was created by this call.</returns>
    Task<EnsuredUserAccount> EnsureUserAccount(
        EnsureUserAccount request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the account bound to the supplied identity provider subject, without refreshing it.
    /// </summary>
    /// <param name="subject">The identity provider subject claim.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The stored account.</returns>
    Task<UserAccount> GetUserAccountBySubject(
        string subject,
        CancellationToken cancellationToken = default);
}
