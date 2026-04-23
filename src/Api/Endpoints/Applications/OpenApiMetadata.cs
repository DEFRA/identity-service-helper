// <copyright file="OpenApiMetadata.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Endpoints.Applications;

public static class OpenApiMetadata
{
    public const string Tag = nameof(RouteNames.Applications);

    // GetAll endpoint
    public static class GetAllRoute
    {
        public const string Name = "GetAllApplications";
        public const string Summary = "Get all applications";
        public const string Description = "Retrieves a list of all applications with optional filtering and pagination";
    }

    // GetById endpoint
    public static class GetByIdRoute
    {
        public const string Name = "GetApplication";
        public const string Summary = "Get application by identifier";
        public const string Description = "Retrieves a specific application by its internal identifier";
    }

    // PutById endpoint
    public static class PutByIdRoute
    {
        public const string Name = "UpdateApplication";
        public const string Summary = "Update an existing application";

        public const string Description =
            "Updates an existing application with the provided data. Requires the operator identifier in headers";
    }

    // Post endpoint
    public static class PostRoute
    {
        public const string Name = "CreateApplication";
        public const string Summary = "Create a new application";

        public const string Description =
            "Creates a new application with the provided data. Requires the operator identifier in headers";
    }

    // DeleteById endpoint
    public static class DeleteByIdRoute
    {
        public const string Name = "DeleteApplication";
        public const string Summary = "Delete an application";

        public const string Description =
            "Deletes an application by its unique identifier. Requires the operator identifier in headers";
    }
}
