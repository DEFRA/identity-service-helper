// <copyright file="OpenApiMetadata.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>
namespace Defra.Identity.Api.Endpoints.Roles;

public class OpenApiMetadata
{
    public const string Tag = nameof(RouteNames.Roles);

    // GetAll endpoint
    public static class GetAllRoute
    {
        public const string Name = "GetAllRoles";
        public const string Summary = "Get all roles";
        public const string Description = "Retrieves a list of all roles";
    }
}
