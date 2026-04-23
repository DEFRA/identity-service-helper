// <copyright file="OpenApiMetadata.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Endpoints.Delegations;

public static class OpenApiMetadata
{
    public const string Tag = nameof(RouteNames.Delegations);

    public static class GetAll
    {
        public const string Name = "GetAllCphDelegations";
        public const string Summary = "Get all CPH delegations";

        public const string Description =
            "Retrieves a list of all CPH (County Parish Holding) delegations for the authenticated operator";
    }

    public static class GetById
    {
        public const string Name = "delegations";
        public const string Summary = "Get CPH delegation by ID";
        public const string Description = "Retrieves a specific CPH delegation by its unique identifier";
    }

    public static class Create
    {
        public const string Name = "CreateCphDelegation";
        public const string Summary = "Create a new CPH delegation";
        public const string Description = "Creates a new CPH delegation request from one operator to another";
    }

    public static class Update
    {
        public const string Name = "UpdateCphDelegation";
        public const string Summary = "Update CPH delegation by ID";
        public const string Description = "Updates an existing CPH delegation by its unique identifier";
    }

    public static class Accept
    {
        public const string Name = "AcceptCphDelegation";
        public const string Summary = "Accept CPH delegation";
        public const string Description = "Accepts a pending CPH delegation request by its unique identifier";
    }

    public static class Reject
    {
        public const string Name = "RejectCphDelegation";
        public const string Summary = "Reject CPH delegation";
        public const string Description = "Rejects a pending CPH delegation request by its unique identifier";
    }

    public static class Revoke
    {
        public const string Name = "RevokeCphDelegation";
        public const string Summary = "Revoke CPH delegation";
        public const string Description = "Revokes an active CPH delegation by its unique identifier";
    }

    public static class Expire
    {
        public const string Name = "ExpireCphDelegation";
        public const string Summary = "Expire CPH delegation";
        public const string Description = "Manually expires a CPH delegation by its unique identifier";
    }

    public static class Delete
    {
        public const string Name = "DeleteCphDelegation";
        public const string Summary = "Delete CPH delegation";
        public const string Description = "Deletes a CPH delegation by its unique identifier";
    }
}
