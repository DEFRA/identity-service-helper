// <copyright file="OpenApiMetadataTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Tests;

public class OpenApiMetadataTests
{
    [Fact]
    public void Verify_Root_Metadata_Text()
    {
        OpenApiMetadata.CountyElement.ShouldBe("County part of the CPH (first 2 digits)");
        OpenApiMetadata.ParishElement.ShouldBe("Parish part of the CPH (middle 3 digits)");
        OpenApiMetadata.HoldingElement.ShouldBe("Holding part of the CPH (last 4 digits)");

        OpenApiMetadata.IncludeInactive.ShouldBe("Whether to include inactive objects in the response");
        OpenApiMetadata.Expired.ShouldBe("Is the object expired");
        OpenApiMetadata.ExpiresAt.ShouldBe("When the object expires at");
    }

    [Fact]
    public void Verify_Application_Metadata_Text()
    {
        OpenApiMetadata.Applications.Id.ShouldBe("The internal identifier of the ApplicationId");
        OpenApiMetadata.Applications.Name.ShouldBe("The name of the application");
        OpenApiMetadata.Applications.TenantName.ShouldBe("The name of the application's tenant");
        OpenApiMetadata.Applications.Description.ShouldBe("The description of the application");
        OpenApiMetadata.Applications.Scopes.ShouldBe("The scopes of the application");
        OpenApiMetadata.Applications.RedirectUris.ShouldBe("The redirect URIs of the application");
        OpenApiMetadata.Applications.Secret.ShouldBe("The secret of the application");
        OpenApiMetadata.Applications.Status.ShouldBe("The status of the application");
    }

    [Fact]
    public void Verify_AnimalSpecies_Metadata_Text()
    {
        OpenApiMetadata.AnimalSpecies.Id.ShouldBe("The internal identifier of the AnimalSpeciesId");
        OpenApiMetadata.AnimalSpecies.Name.ShouldBe("The name of the animal species");
        OpenApiMetadata.AnimalSpecies.IsActive.ShouldBe("Is the animal species enabled");
    }

    [Fact]
    public void Verify_Cphs_Metadata_Text()
    {
        OpenApiMetadata.Cphs.Id.ShouldBe("The internal identifier of the CphId");
        OpenApiMetadata.Cphs.ExpiredDescription.ShouldBe("Whether expired objects should be included in the response");
        OpenApiMetadata.Cphs.CphNumber.ShouldBe("The County Parish Holder number");
    }

    [Fact]
    public void Verify_Delegations_Metadata_Text()
    {
        OpenApiMetadata.Delegations.Id.ShouldBe("The internal identifier of the DelegationId");
        OpenApiMetadata.Delegations.DelegatingUserId.ShouldBe("The internal identifier of the DelegatingUserId");
        OpenApiMetadata.Delegations.DelegatingUserName.ShouldBe("The delegating user's name");
        OpenApiMetadata.Delegations.DelegatedUserId.ShouldBe("The internal identifier of the DelegatedUserId");
        OpenApiMetadata.Delegations.DelegatedUserName.ShouldBe("The delegated user's name");
        OpenApiMetadata.Delegations.DelegatedUserRoleId.ShouldBe("The internal identifier of the DelegatedUserRoleId");
        OpenApiMetadata.Delegations.DelegatedUserRoleName.ShouldBe("The role of the delegated user");
        OpenApiMetadata.Delegations.DelegatedUserEmail.ShouldBe("The delegated user's email address");
        OpenApiMetadata.Delegations.InvitationExpiresAt.ShouldBe("The invitation expiration date");
        OpenApiMetadata.Delegations.InvitationAcceptedAt.ShouldBe("The invitation acceptance date");
        OpenApiMetadata.Delegations.InvitationRejectedAt.ShouldBe("The invitation rejection date");
        OpenApiMetadata.Delegations.RevokedAt.ShouldBe("The revocation date");
        OpenApiMetadata.Delegations.RevokedByName.ShouldBe("The revoker's name");
    }

    [Fact]
    public void Verify_Paging_Metadata_Text()
    {
        OpenApiMetadata.Paging.PageNumber.ShouldBe("The page number to return (1-based)");
        OpenApiMetadata.Paging.PageSize.ShouldBe("The number of items to return per page");
        OpenApiMetadata.Paging.OrderByProperty.ShouldBe("The property to order by");
        OpenApiMetadata.Paging.OrderByDescending.ShouldBe("Whether to order by descending");
        OpenApiMetadata.Paging.Items.ShouldBe("The collection of items in the page");
        OpenApiMetadata.Paging.TotalCount.ShouldBe("The number of items in the result set");
        OpenApiMetadata.Paging.TotalPages.ShouldBe("The number of pages in the result set");
    }

    [Fact]
    public void Verify_AssociatedUsers_Metadata_Text()
    {
        OpenApiMetadata.AssociatedUsers.Id.ShouldBe("The internal identifier of the AssociatedUserId");
    }

    [Fact]
    public void Verify_Users_Metadata_Text()
    {
        OpenApiMetadata.Users.Id.ShouldBe("The internal identifier of the UserId");
        OpenApiMetadata.Users.Email.ShouldBe("The email address of the user");
        OpenApiMetadata.Users.DisplayName.ShouldBe("The displayed name of the user");
        OpenApiMetadata.Users.FirstName.ShouldBe("The first name of the user");
        OpenApiMetadata.Users.LastName.ShouldBe("The last name of the user");
    }

    [Fact]
    public void Verify_Roles_Metadata_Text()
    {
        OpenApiMetadata.Roles.Id.ShouldBe("The internal identifier of the RoleId");
        OpenApiMetadata.Roles.Name.ShouldBe("The name of the role");
    }
}
