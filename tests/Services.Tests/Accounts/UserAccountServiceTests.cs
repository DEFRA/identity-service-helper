// <copyright file="UserAccountServiceTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Tests.Accounts;

using Defra.Identity.KeeperReferenceData.Providers;
using Defra.Identity.Models.Requests.UserAccounts.Commands;
using Defra.Identity.Repositories.Common.Exceptions;
using Defra.Identity.Services.UserAccounts;
using NSubstitute;
using KrdsCphAssociation = Defra.Identity.KeeperReferenceData.Models.UserAccounts.CphAssociation;
using KrdsEnsureUserAccountRequest = Defra.Identity.KeeperReferenceData.Models.UserAccounts.EnsureUserAccountRequest;
using KrdsEnsureUserAccountResult = Defra.Identity.KeeperReferenceData.Models.UserAccounts.EnsureUserAccountResult;
using KrdsUserAccount = Defra.Identity.KeeperReferenceData.Models.UserAccounts.UserAccount;

public class UserAccountServiceTests
{
    private readonly IUserAccountsProvider provider = Substitute.For<IUserAccountsProvider>();

    [Fact]
    public async Task EnsureUserAccount_Forwards_The_Claims_And_Maps_The_Response()
    {
        var sut = new UserAccountService(provider);
        provider
            .EnsureUserAccount(Arg.Any<KrdsEnsureUserAccountRequest>(), Arg.Any<CancellationToken>())
            .Returns(new KrdsEnsureUserAccountResult(Account(), true));

        var result = await sut.EnsureUserAccount(
            new EnsureUserAccount
            {
                Sub = "subject",
                Email = "jane.farmer@example.com",
                GivenName = "Jane",
                FamilyName = "Farmer",
            },
            TestContext.Current.CancellationToken);

        await provider.Received(1).EnsureUserAccount(
            Arg.Is<KrdsEnsureUserAccountRequest>(r =>
                r.Sub == "subject" &&
                r.Email == "jane.farmer@example.com" &&
                r.GivenName == "Jane" &&
                r.FamilyName == "Farmer"),
            Arg.Any<CancellationToken>());

        Assert.True(result.Created);
        Assert.Equal("subject", result.Account.Subject);
        Assert.Equal("Jane Farmer", result.Account.DisplayName);

        var association = Assert.Single(result.Account.CphAssociations);
        Assert.Equal("22/001/0001", association.CphNumber);
        Assert.Equal("owner", association.Role);
        Assert.Equal("Hill Farm", association.HoldingName);
    }

    [Fact]
    public async Task EnsureUserAccount_Reports_An_Existing_Account_As_Not_Created()
    {
        var sut = new UserAccountService(provider);
        provider
            .EnsureUserAccount(Arg.Any<KrdsEnsureUserAccountRequest>(), Arg.Any<CancellationToken>())
            .Returns(new KrdsEnsureUserAccountResult(Account(), false));

        var result = await sut.EnsureUserAccount(
            new EnsureUserAccount
            {
                Sub = "subject",
                Email = "jane.farmer@example.com",
                GivenName = "Jane",
                FamilyName = "Farmer",
            },
            TestContext.Current.CancellationToken);

        Assert.False(result.Created);
    }

    [Fact]
    public async Task GetUserAccountBySubject_Maps_The_Stored_Account()
    {
        var sut = new UserAccountService(provider);
        provider
            .GetUserAccountBySubject("subject", Arg.Any<CancellationToken>())
            .Returns(Account());

        var account = await sut.GetUserAccountBySubject("subject", TestContext.Current.CancellationToken);

        Assert.Equal("jane.farmer@example.com", account.Email);
        Assert.Single(account.CphAssociations);
    }

    [Fact]
    public async Task GetUserAccountBySubject_Throws_When_The_Subject_Is_Not_Recognised()
    {
        var sut = new UserAccountService(provider);
        provider
            .GetUserAccountBySubject("unknown", Arg.Any<CancellationToken>())
            .Returns((KrdsUserAccount?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => sut.GetUserAccountBySubject("unknown", TestContext.Current.CancellationToken));
    }

    private static KrdsUserAccount Account()
    {
        return new KrdsUserAccount
        {
            Id = "account-id",
            Subject = "subject",
            Email = "jane.farmer@example.com",
            FirstName = "Jane",
            LastName = "Farmer",
            DisplayName = "Jane Farmer",
            AssociationsRefreshedDate = DateTimeOffset.UtcNow,
            CphAssociations =
            [
                new KrdsCphAssociation
                {
                    Id = "cph-identifier-1",
                    CphNumber = "22/001/0001",
                    Role = "owner",
                    PartyId = "party-1",
                    HoldingId = "holding-1",
                    HoldingName = "Hill Farm",
                },
            ],
        };
    }
}
