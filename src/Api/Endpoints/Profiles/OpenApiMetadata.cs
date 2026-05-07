// <copyright file="OpenApiMetadata.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Endpoints.Profiles;

public static class OpenApiMetadata
{
    // GetUsersById endpoint
    public static class GetCphAssignmentsByIdRoute
    {
        public const string Name = "GetCountyParishHoldingUsersById";
        public const string Summary = "Get users for a County Parish Holding by its internal identitifer";

        public const string Description =
            "Retrieves a paged list of users associated with a specific County Parish Holding by its internal identifier";
    }

    // GetUsersByNumber endpoint
    public static class GetCphAssignmentsByCphNumberRoute
    {
        public const string Name = "GetCountyParishHoldingUsersByNumber";

        public const string Summary = "Get users for a County Parish Holding by its County Parish Holding identifier";

        public const string Description =
            "Retrieves a paged list of users associated with a specific Holding by its County Parish Holding identifier";
    }
}
