// <copyright file="Assertions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Tests;

using System.Diagnostics.CodeAnalysis;
using Defra.Identity.Messaging;
using Defra.Identity.Messaging.Models.Request;
using Defra.Identity.Messaging.Services;
using Defra.Identity.Models.Responses.Applications;
using Defra.Identity.Models.Responses.Assignments;
using Defra.Identity.Models.Responses.Cphs;
using Defra.Identity.Models.Responses.Delegations;
using Defra.Identity.Models.Responses.Roles;
using Defra.Identity.Models.Responses.Users;
using Defra.Identity.Postgres.Database.Entities;
using NSubstitute;
using AnimalSpeciesResponse = Defra.Identity.Models.Responses.Species.AnimalSpecies;

[ExcludeFromCodeCoverage]
public static class Assertions
{
    public static Action<Application> ShouldMapFromEntity(Postgres.Database.Entities.Applications entity) =>
        result =>
        {
            result.Id.ShouldBe(entity.ClientId);
            result.Name.ShouldBe(entity.Name);
            result.Description.ShouldBe(entity.Description);
            result.TenantName.ShouldBe(entity.TenantName);
            result.Scopes.ShouldBe(entity.Scopes.Split(";"));
            result.RedirectUris.ShouldBe(entity.RedirectUris.Split(";"));
            result.Secret.ShouldBe(entity.Secret);
        };

    public static Action<User> ShouldMapFromEntity(UserAccounts entity) =>
        result =>
        {
            result.Id.ShouldBe(entity.Id);
            result.Email.ShouldBe(entity.EmailAddress);
            result.FirstName.ShouldBe(entity.FirstName);
            result.LastName.ShouldBe(entity.LastName);
            result.DisplayName.ShouldBe(entity.DisplayName);
            result.Active.ShouldBe(entity.DeletedAt == null);
        };

    public static Action<Role> ShouldMapFromEntity(Postgres.Database.Entities.Roles entity) =>
        result =>
        {
            result.Id.ShouldBe(entity.Id);
            result.Name.ShouldBe(entity.Name);
            result.Description.ShouldBe(entity.Description);
        };

    public static Action<AnimalSpeciesResponse> ShouldMapFromEntity(AnimalSpecies entity) =>
        result =>
        {
            result.Id.ShouldBe(entity.Id);
            result.Name.ShouldBe(entity.Name);
            result.IsActive.ShouldBe(entity.IsActive);
        };

    public static Action<Cph> ShouldMapFromEntity(CountyParishHoldings entity) =>
        result =>
        {
            result.Id.ShouldBe(entity.Id);
            result.CountyParishHoldingNumber.ShouldBe(entity.Identifier);
            result.ExpiredAt.ShouldBe(entity.ExpiredAt);
            result.Expired.ShouldBe(entity.ExpiredAt != null);

            result.AllowedSpecies.Count().ShouldBe(entity.CountyParishHoldingAnimalSpecies.Count);

            foreach (var allowedSpecies in result.AllowedSpecies.Index())
            {
                var associatedAllowedSpeciesEntity =
                    entity.CountyParishHoldingAnimalSpecies.ToArray()[allowedSpecies.Index];

                allowedSpecies.Item.ShouldSatisfyAllConditions(
                    Assertions.ShouldMapFromEntity(associatedAllowedSpeciesEntity.AnimalSpecies));
            }
        };

    public static Action<CphAssignment> ShouldMapFromEntity(UserAccountCountyParishHoldingAssignments entity) =>
        result =>
        {
            result.Id.ShouldBe(entity.Id);
            result.CountyParishHoldingId.ShouldBe(entity.CountyParishHolding.Id);
            result.CountyParishHoldingNumber.ShouldBe(entity.CountyParishHolding.Identifier);
            result.UserId.ShouldBe(entity.UserAccountId);
            result.RoleId.ShouldBe(entity.RoleId);
            result.RoleName.ShouldBe(entity.Role.Name);
            result.Email.ShouldBe(entity.UserAccount.EmailAddress);
            result.DisplayName.ShouldBe(entity.UserAccount.DisplayName);
        };

    public static Action<CphDelegation> ShouldMapFromEntity(CountyParishHoldingDelegations entity) =>
        result =>
        {
            result.Id.ShouldBe(entity.Id);
            result.CountyParishHoldingId.ShouldBe(entity.CountyParishHolding.Id);
            result.CountyParishHoldingNumber.ShouldBe(entity.CountyParishHolding.Identifier);
            result.DelegatingUserId.ShouldBe(entity.DelegatingUserId);
            result.DelegatingUserName.ShouldBe(entity.DelegatingUser.DisplayName);
            result.DelegatedUserId.ShouldBe(entity.DelegatedUserId);
            result.DelegatedUserName.ShouldBe(entity.DelegatedUser?.DisplayName);
            result.DelegatedUserEmail.ShouldBe(entity.DelegatedUserEmail);
            result.DelegatedUserRoleId.ShouldBe(entity.DelegatedUserRoleId);
            result.DelegatedUserRoleName.ShouldBe(entity.DelegatedUserRole.Name);
            result.InvitationExpiresAt.ShouldBe(entity.InvitationExpiresAt);
            result.InvitationAcceptedAt.ShouldBe(entity.InvitationAcceptedAt);
            result.InvitationRejectedAt.ShouldBe(entity.InvitationRejectedAt);
            result.RevokedAt.ShouldBe(entity.RevokedAt);
            result.RevokedById.ShouldBe(entity.RevokedById);
            result.RevokedByName.ShouldBe(entity.RevokedByUser?.DisplayName);
            result.ExpiresAt.ShouldBe(entity.ExpiresAt);
            result.Active.ShouldBe(IsActiveDelegation(entity));
        };

    public static Action<IMessagingFactory> ShouldHaveQueuedDelegationEmailMessage(
        Guid delegationId,
        string emailAddress,
        MessageTemplateTypes.Delegation template) =>
        (messagingFactory) =>
        {
            messagingFactory.Received(1).QueueDelegationEmailAsync(
                Arg.Is<DelegationEmailMessage>(message =>
                    message.CphDelegationId == delegationId && message.TemplateId == template.Value &&
                    message.Recipient == emailAddress),
                Arg.Any<CancellationToken>());
        };

    private static bool IsActiveDelegation(CountyParishHoldingDelegations entity)
    {
        var hasValidReferences = entity.CountyParishHolding.DeletedAt == null &&
                                 entity.CountyParishHolding.ExpiredAt == null &&
                                 entity.DelegatingUser.DeletedAt == null &&
                                 entity.DelegatedUser is { DeletedAt: null };

        var hasAtLeastOneActiveCphOwnerAssignment = entity.CountyParishHolding
            .ApplicationUserAccountHoldingAssignments.Any(assignment =>
                assignment.DeletedAt == null && assignment.UserAccount.DeletedAt == null);

        var isDeleted = entity.DeletedAt != null;
        var isExpired = entity.ExpiresAt != null && DateTime.UtcNow < entity.ExpiresAt;
        var isRejectedOrRevoked = entity.InvitationRejectedAt != null || entity.RevokedAt != null;
        var isAccepted = entity.InvitationAcceptedAt != null;

        return hasValidReferences && hasAtLeastOneActiveCphOwnerAssignment && !isExpired && !isDeleted &&
               !isRejectedOrRevoked && isAccepted;
    }
}
