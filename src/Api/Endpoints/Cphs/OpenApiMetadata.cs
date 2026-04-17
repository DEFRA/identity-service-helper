// <copyright file="OpenApiMetadata.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Endpoints.Cphs;

public static class OpenApiMetadata
{
    public const string Tag = nameof(RouteNames.CountyParishHoldings);

    // GetAllPaged endpoint
    public static class GetAllPagedRoute
    {
        public const string Name = "GetAllCountyParishHoldings";
        public const string Summary = "Get all County Parish Holdings";
        public const string Description = "Retrieves a paged list of all County Parish Holdings";
    }

    // GetById endpoint
    public static class GetByIdRoute
    {

        public const string Name = "GetCountyParishHoldingById";
        public const string Summary = "Get a County Parish Holding by its internal identifier";

        public const string Description =
            "Retrieves a specific County Parish Holding by its internal identifier";
    }

    // GetByNumber endpoint
    public static class GetByNumberRoute
    {
        public const string Name = "GetCountyParishHoldingByNumber";
        public const string Summary = "Get a County Parish Holding by its County Parish Holding identifier";

        public const string Description =
            "Retrieves a specific County Parish Holding by its County Parish Holding identifier";
    }

    // ExpireById endpoint
    public static class ExpireByIdRoute
    {
        public const string Name = "ExpireCountyParishHoldingById";
        public const string Summary = "Expire a County Parish Holding by its internal identifier";

        public const string Description =
            "Marks a specific County Parish Holding as expired by its internal identifier";
    }

    // ExpireByNumber endpoint
    public static class ExpireByNumberRoute
    {
        public const string Name = "ExpireCountyParishHoldingByNumber";

        public const string Summary =
            "Expire a County Parish Holding by its County Parish Holding identifier";

        public const string Description =
            "Marks a specific County Parish Holding as expired by its County Parish Holding identifier";
    }

    // DeleteById endpoint
    public static class DeleteByIdRoute
    {
        public const string Name = "DeleteCountyParishHoldingById";
        public const string Summary = "Delete a County Parish Holding by its internal identitifer";

        public const string Description =
            "Deletes a specific County Parish Holding by its internal identitifer";
    }

    // DeleteByNumber endpoint
    public static class DeleteByNumberRoute
    {
        public const string Name = "DeleteCountyParishHoldingByNumber";

        public const string Summary =
            "Delete a County Parish Holding by its County Parish Holding identifier";

        public const string Description =
            "Deletes a specific County Parish Holding by its County Parish Holding identifier";
    }

    // GetUsersById endpoint
    public static class GetUsersByIdRoute
    {
        public const string Name = "GetCountyParishHoldingUsersById";
        public const string Summary = "Get users for a County Parish Holding by its internal identitifer";

        public const string Description =
            "Retrieves a paged list of users associated with a specific County Parish Holding by its internal identifier";
    }

    // GetUsersByNumber endpoint
    public static class GetUsersByNumberRoute
    {
        public const string Name = "GetCountyParishHoldingUsersByNumber";

        public const string Summary = "Get users for a County Parish Holding by its County Parish Holding identifier";

        public const string Description =
            "Retrieves a paged list of users associated with a specific Holding by its County Parish Holding identifier";
    }
}
