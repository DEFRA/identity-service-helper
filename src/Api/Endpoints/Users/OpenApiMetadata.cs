// <copyright file="OpenApiMetadata.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Endpoints.Users;

public static class OpenApiMetadata
{
    public const string Tag = nameof(RouteNames.Users);

    public static class GetAllRoute
    {
        public const string Name = "GetAllUsers";
        public const string Summary = "Get all users";
        public const string Description = "Retrieves a list of all users in the system";
    }

    public static class GetByIdRoute
    {
        public const string Name = "GetUser";
        public const string Summary = "Get user by its internal identifier";
        public const string Description = "Retrieves a specific user by its internal identifier";
    }

    public static class PutByIdRoute
    {
        public const string Name = "UpdateUser";
        public const string Summary = "Update user";
        public const string Description = "Updates an existing in the system";
    }

    public static class PostRoute
    {
        public const string Name = "CreateUser";
        public const string Summary = "Create user";
        public const string Description = "Creates a new user in the system";
    }

    public static class DeleteByIdRoute
    {
        public const string Name = "DeleteUser";
        public const string Summary = "Delete user";
        public const string Description = "Deletes a specific user by its internal identifier";
    }
}
