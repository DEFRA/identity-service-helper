// <copyright file="ApplicationService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Applications;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Applications;
using Defra.Identity.Repositories.Common.Exceptions;
using Defra.Identity.Requests.Applications.Commands.Create;
using Defra.Identity.Requests.Applications.Commands.Update;
using Defra.Identity.Requests.Applications.Queries;
using Defra.Identity.Responses.Applications;
using Microsoft.Extensions.Logging;

public class ApplicationService : IApplicationService
{
    private const string Separator = ";";
    private readonly IApplicationsRepository repository;
    private readonly ILogger<ApplicationService> logger;

    public ApplicationService(IApplicationsRepository repository, ILogger<ApplicationService> logger)
    {
        this.repository = repository;
        this.logger = logger;
    }

    public async Task<List<Application>> GetAll(GetApplications request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting all applications");
        var applicationEntities = await repository.GetList(x => true, cancellationToken);

        var applications = applicationEntities.Select(app => new Application()
        {
            Id = app.ClientId,
            Name = app.Name,
            TenantName = app.TenantName,
            Description = app.Description,
            Secret = app.Secret,
            Scopes = app.Scopes.Split(Separator, StringSplitOptions.RemoveEmptyEntries).ToList(),
            RedirectUri = app.RedirectUris.Split(Separator, StringSplitOptions.RemoveEmptyEntries).ToList(),
        }).ToList();

        return applications;
    }

    public async Task<Application> Get(GetApplicationById request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting application by id {Id}", request.Id);
        Expression<Func<Applications, bool>> filter = x => x.ClientId == request.Id;

        var application = await repository.GetSingle(filter, cancellationToken);

        if (application == null)
        {
            logger.LogWarning("Application with id {Id} not found", request.Id);
            throw new NotFoundException("Application not found.");
        }

        return new Application()
        {
            Id = application.ClientId,
            Name = application.Name,
            TenantName = application.TenantName,
            Secret = application.Secret,
            Description = application.Description,
            Scopes = application.Scopes.Split(Separator, StringSplitOptions.RemoveEmptyEntries).ToList(),
            RedirectUri = application.RedirectUris.Split(Separator, StringSplitOptions.RemoveEmptyEntries).ToList(),
        };
    }

    public async Task<Application> Update(UpdateApplication application, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating application with id {Id}", application.Id);
        var existingApplication = await repository.GetSingle(x => x.ClientId.Equals(application.Id), cancellationToken);

        if (existingApplication == null)
        {
            logger.LogWarning("Application with id {Id} not found for update", application.Id);
            throw new NotFoundException($"Application with id {application.Id} not found.");
        }

        existingApplication.Name = application.Name;
        existingApplication.TenantName = application.TenantName;
        existingApplication.Description = application.Description;
        existingApplication.Scopes = string.Join(Separator, application.Scopes);
        existingApplication.RedirectUris = string.Join(Separator, application.RedirectUris);
        existingApplication.Secret = application.Secret;

        var updated = await repository.Update(existingApplication, cancellationToken);

        return new Application
        {
            Id = updated.ClientId,
            Name = updated.Name,
            TenantName = updated.TenantName,
            Description = updated.Description,
            Secret = updated.Secret,
            Scopes = updated.Scopes.Split(Separator, StringSplitOptions.RemoveEmptyEntries).ToList(),
            RedirectUri = updated.RedirectUris.Split(Separator, StringSplitOptions.RemoveEmptyEntries).ToList(),
        };
    }

    public async Task<Application> Create(CreateApplication application, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating new application with name {Name}", application.Name);

        var newApplication = new Applications
        {
            Name = application.Name,
            ClientId = application.Id,
            TenantName = application.TenantName,
            Description = application.Description,
            CreatedById = application.OperatorId,
            Scopes = string.Join(Separator, application.Scopes),
            Secret = application.Secret,
            RedirectUris = string.Join(Separator, application.RedirectUris),
        };

        var createdApplication = await repository.Create(newApplication, cancellationToken);
        return new Application
        {
            Id = createdApplication.ClientId,
            Name = createdApplication.Name,
            TenantName = createdApplication.TenantName,
            Description = createdApplication.Description,
            Scopes = createdApplication.Scopes.Split(Separator, StringSplitOptions.RemoveEmptyEntries).ToList(),
            RedirectUri = createdApplication.RedirectUris.Split(Separator, StringSplitOptions.RemoveEmptyEntries).ToList(),
        };
    }

    public async Task<bool> Delete(Guid id, Guid operatorId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting application with id {Id} by operator {OperatorId}", id, operatorId);
        return await repository.Delete(x => x.ClientId.Equals(id), operatorId, cancellationToken);
    }
}
