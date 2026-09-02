// <copyright file="OpenApiMetadata.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Endpoints.UserAccounts;

public static class OpenApiMetadata
{
    public const string Tag = "UserAccounts";

    public static class EnsureRoute
    {
        public const string Name = "EnsureUserAccount";
        public const string Summary = "Ensure a user account exists and refresh it from identity provider claims";

        public const string Description =
            "Called on every successful logon. Ensures the keeper records data service holds an account for the supplied subject, refreshes the profile from the claims and rebuilds the County Parish Holding associations from the SAM mastered party data";
    }

    public static class GetBySubjectRoute
    {
        public const string Name = "GetUserAccountBySubject";
        public const string Summary = "Get a user account by identity provider subject";

        public const string Description =
            "Retrieves the stored user account, including the County Parish Holding associations captured by the most recent logon. Does not trigger a refresh";
    }
}
