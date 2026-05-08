// <copyright file="AnimalSpeciesRepository.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Species;

using Microsoft.Extensions.Logging;

public partial class AnimalSpeciesRepository
{
    [LoggerMessage(LogLevel.Information, "Getting single animal species")]
    partial void LogGettingSingleAnimalSpecies();

    [LoggerMessage(LogLevel.Information, "Getting list of animal species")]
    partial void LogGettingListOfAnimalSpecies();

    [LoggerMessage(LogLevel.Information, "Updating animal species with id {Id}")]
    partial void LogUpdatingAnimalSpeciesWithId(string id);
}
