// <copyright file="AnimalSpeciesService.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Species;

using Microsoft.Extensions.Logging;

public partial class AnimalSpeciesService
{
    [LoggerMessage(LogLevel.Information, "Getting all animal species, includeHidden: {IncludeHidden}")]
    partial void LogGettingAllAnimalSpeciesIncludeHidden(string includeHidden);

    [LoggerMessage(LogLevel.Information, "Getting animal species by id {Id}")]
    partial void LogGettingAnimalSpeciesById(string id);

    [LoggerMessage(LogLevel.Warning, "Animal species with id {Id} not found")]
    partial void LogAnimalSpeciesWithIdNotFound(string id);

    [LoggerMessage(LogLevel.Information, "Updating animal species with id {Id}")]
    partial void LogUpdatingAnimalSpeciesWithId(string id);

    [LoggerMessage(LogLevel.Warning, "Animal species with id {Id} not found for update")]
    partial void LogAnimalSpeciesWithIdNotFoundForUpdate(string id);
}
