// <copyright file="UserAccountService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.UserAccounts;

using Defra.Identity.KeeperReferenceData.Providers;
using Defra.Identity.Models.Requests.UserAccounts.Commands;
using Defra.Identity.Models.Responses.UserAccounts;
using Defra.Identity.Repositories.Common.Exceptions;
using Defra.Identity.Services.Common.Mappers;
using KrdsEnsureUserAccountRequest = Defra.Identity.KeeperReferenceData.Models.UserAccounts.EnsureUserAccountRequest;

public class UserAccountService(IUserAccountsProvider provider) : IUserAccountService
{
    public async Task<EnsuredUserAccount> EnsureUserAccount(
        EnsureUserAccount request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var result = await provider.EnsureUserAccount(
            new KrdsEnsureUserAccountRequest
            {
                Sub = request.Sub,
                Email = request.Email,
                GivenName = request.GivenName,
                FamilyName = request.FamilyName,
            },
            cancellationToken);

        return new EnsuredUserAccount(
            UserAccountMapper.MapKrdsUserAccountToUserAccount(result.Account),
            result.Created);
    }

    public async Task<UserAccount> GetUserAccountBySubject(
        string subject,
        CancellationToken cancellationToken = default)
    {
        var account = await provider.GetUserAccountBySubject(subject, cancellationToken)
                      ?? throw new NotFoundException($"User account with subject {subject} not found");

        return UserAccountMapper.MapKrdsUserAccountToUserAccount(account);
    }
}
