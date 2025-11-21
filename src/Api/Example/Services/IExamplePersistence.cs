// <copyright file="IExamplePersistence.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Example.Services;

using Defra.Identity.Api.Example.Models;

public interface IExamplePersistence
{
    public Task<bool> CreateAsync(ExampleModel example);

    public Task<ExampleModel?> GetByExampleName(string name);

    public Task<IEnumerable<ExampleModel>> GetAllAsync();

    public Task<IEnumerable<ExampleModel>> SearchByValueAsync(string searchTerm);

    public Task<bool> UpdateAsync(ExampleModel example);

    public Task<bool> DeleteAsync(string name);
}
