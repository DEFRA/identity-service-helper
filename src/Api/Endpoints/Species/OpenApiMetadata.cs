// <copyright file="OpenApiMetadata.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Endpoints.Species;

public static class OpenApiMetadata
{
    public const string Tag = nameof(RouteNames.AnimalSpecies);

    public static class GetAllRoute
    {
        public const string Name = "GetAllAnimalSpecies";
        public const string Summary = "Get all animal species";

        public const string Description =
            "Retrieves a list of all Animal Species. The list with exclude the Animal Species that are not enabled unless specified otherwise.";
    }

    public static class GetByIdRoute
    {
        public const string Name = "GetAnimalSpecies";
        public const string Summary = "Get animal species by ID";
        public const string Description = "Retrieves a specific Animal Species by its unique identifier.";
    }

    public static class ToggleByIdRoute
    {
        public const string Name = "ToggleAnimalSpecies";
        public const string Summary = "Toggle animal species enabled status";

        public const string Description =
            "Toggles the enabled/disabled status of a specific Animal Species by its unique identifier.";
    }
}
