// <copyright file="OpenApiMetadata.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models;

public static class OpenApiMetadata
{
    public const string CountyElement = "County part of the CPH (first 2 digits)";
    public const string ParishElement = "Parish part of the CPH (middle 3 digits)";
    public const string HoldingElement = "Holding part of the CPH (last 4 digits)";

    private const string Id = "The internal identifier of the ";
    public const string IncludeInactive = "Whether to include inactive objects in the response";
    public const string Expired = "Is the object expired";
    public const string ExpiresAt = "When the object expires at";

    public static class Applications
    {
        public const string Id = $"{OpenApiMetadata.Id} ApplicationId";
        public const string Name = "The name of the application";
        public const string TenantName = "The name of the application's tenant";
        public const string Description = "The description of the application";
        public const string Scopes = "The scopes of the application";
        public const string RedirectUris = "The redirect URIs of the application";
        public const string Secret = "The secret of the application";
        public const string Status = "The status of the application";
    }

    public static class AnimalSpecies
    {
        public const string Id = $"{OpenApiMetadata.Id} AnimalSpeciesId";
        public const string Name = "The name of the animal species";
        public const string IsActive = "Is the animal species enabled";
    }

    public static class Cphs
    {
        public const string Id = $"{OpenApiMetadata.Id} CphId";
        public const string ExpiredDescription = "Whether expired objects should be included in the response";
        public const string CphNumber = "The County Parish Holder number";
    }

    public static class Delegations
    {
        public const string Id = $"{OpenApiMetadata.Id} DelegationId";
        public const string DelegatingUserId = $"{OpenApiMetadata.Id} DelegatingUserId";
        public const string DelegatingUserName = "The delegating user's name";
        public const string DelegatedUserId = $"{OpenApiMetadata.Id} DelegatedUserId";
        public const string DelegatedUserName = "The delegated user's name";
        public const string DelegatedUserRoleId = $"{OpenApiMetadata.Id} DelegatedUserRoleId";
        public const string DelegatedUserRoleName = "The role of the delegated user";
        public const string DelegatedUserEmail = "The delegated user's email address";
        public const string InvitationExpiresAt = "The invitation expiration date";
        public const string InvitationAcceptedAt = "The invitation acceptance date";
        public const string InvitationRejectedAt = "The invitation rejection date";
        public const string RevokedAt = "The revocation date";
        public const string RevokedByName = "The revoker's name";
    }

    public static class Paging
    {
        public const string PageNumber = "The page number to return (1-based)";
        public const string PageSize = "The number of items to return per page";
        public const string OrderByProperty = "The property to order by";
        public const string OrderByDescending = "Whether to order by descending";
        public const string Items = "The collection of items in the page";
        public const string TotalCount = "The number of items in the result set";
        public const string TotalPages = "The number of pages in the result set";
    }

    public static class AssociatedUsers
    {
        public const string Id = $"{OpenApiMetadata.Id} AssociatedUserId";
    }

    public static class Users
    {
        public const string Id = $"{OpenApiMetadata.Id} UserId";
        public const string Email = "The email address of the user";
        public const string DisplayName = "The displayed name of the user";
        public const string FirstName = "The first name of the user";
        public const string LastName = "The last name of the user";
    }

    public static class Roles
    {
        public const string Id = $"{OpenApiMetadata.Id} RoleId";
        public const string Name = "The name of the role";
    }
}
