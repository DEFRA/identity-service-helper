// <copyright file="UserAccountsProvider.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Providers;

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Defra.Identity.KeeperReferenceData.Models.UserAccounts;
using Microsoft.Extensions.Logging;

public partial class UserAccountsProvider(HttpClient client, ILogger<UserAccountsProvider> logger)
    : IUserAccountsProvider
{
    private const string UserAccountsPath = "v2/user-accounts";

    public async Task<EnsureUserAccountResult> EnsureUserAccount(
        EnsureUserAccountRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        LogEnsuringUserAccount();

        using var response = await client.PostAsJsonAsync(UserAccountsPath, request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var account = await ReadAccount(response, cancellationToken);

        return new EnsureUserAccountResult(account, response.StatusCode == HttpStatusCode.Created);
    }

    public async Task<UserAccount?> GetUserAccountBySubject(string subject, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(subject);

        LogGettingUserAccountBySubject();

        using var response = await client.GetAsync($"{UserAccountsPath}/{Uri.EscapeDataString(subject)}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await ReadAccount(response, cancellationToken);
    }

    private async Task<UserAccount> ReadAccount(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        try
        {
            return JsonSerializer.Deserialize<UserAccount>(body)
                   ?? throw new JsonException("The user account response body was empty.");
        }
        catch (JsonException jsonException)
        {
            LogErrorDeserializingUserAccountBodyResponsebody(body, jsonException);
            throw;
        }
    }
}
