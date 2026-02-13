// <copyright file="ApplicationService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Applications;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Applications;
using Defra.Identity.Repositories.Exceptions;
using Defra.Identity.Requests.Applications.Commands.Create;
using Defra.Identity.Requests.Applications.Commands.Update;
using Defra.Identity.Requests.Applications.Queries;
using Defra.Identity.Responses.Applications;
using Microsoft.Extensions.Logging;

public class ApplicationService : IApplicationService
{
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
            Id = app.Id,
            Name = app.Name,
            ClientId = app.ClientId,
            TenantName = app.TenantName,
            Description = app.Description,
        }).ToList();

        return applications;
    }

    public async Task<Application> Get(GetApplicationById request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting application by id {Id}", request.Id);
        Expression<Func<Applications, bool>> filter = x => x.Id == request.Id;

        var application = await repository.GetSingle(filter, cancellationToken);

        if (application == null)
        {
            logger.LogWarning("Application with id {Id} not found", request.Id);
            throw new NotFoundException("Application not found.");
        }

        return new Application()
        {
            Id = application.Id,
            Name = application.Name,
            ClientId = application.ClientId,
            TenantName = application.TenantName,
            Description = application.Description,
        };
    }

    public async Task<Application> Update(UpdateApplication application, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating application with id {Id}", application.Id);
        var existingApplication = await repository.GetSingle(x => x.Id.Equals(application.Id), cancellationToken);

        if (existingApplication == null)
        {
            logger.LogWarning("Application with id {Id} not found for update", application.Id);
            throw new NotFoundException($"Application with id {application.Id} not found.");
        }

        existingApplication.Name = application.Name;
        existingApplication.ClientId = application.ClientId;
        existingApplication.TenantName = application.TenantName;
        existingApplication.Description = application.Description;

        var updated = await repository.Update(existingApplication, cancellationToken);

        return new Application
        {
            Id = updated.Id,
            Name = updated.Name,
            ClientId = updated.ClientId,
            TenantName = updated.TenantName,
            Description = updated.Description,
        };
    }

    public async Task<Application> Create(CreateApplication application, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating new application with name {Name}", application.Name);
        var newApplication = new Applications
        {
            Name = application.Name,
            ClientId = application.ClientId,
            TenantName = application.TenantName,
            Description = application.Description,
            CreatedById = application.OperatorId,
        };

        var createdApplication = await repository.Create(newApplication, cancellationToken);
        return new Application()
        {
            Id = createdApplication.Id,
            Name = createdApplication.Name,
            ClientId = createdApplication.ClientId,
            TenantName = createdApplication.TenantName,
            Description = createdApplication.Description,
        };
    }

    public async Task<bool> Delete(Guid id, Guid operatorId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting application with id {Id} by operator {OperatorId}", id, operatorId);
        return await repository.Delete(x => x.Id == id, operatorId, cancellationToken);
    }
}
