// <copyright file="ApplicationService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Applications;

using System.Linq.Expressions;
using Defra.Identity.Models.Requests.Applications.Commands;
using Defra.Identity.Models.Requests.Applications.Queries;
using Defra.Identity.Models.Responses.Applications;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Applications;
using Defra.Identity.Repositories.Common.Exceptions;
using Microsoft.Extensions.Logging;

public partial class ApplicationService(
    IApplicationsRepository repository,
    ILogger<ApplicationService> logger)
    : IApplicationService
{
    private const string Separator = ";";

    public async Task<List<Application>> GetAll(
        GetApplications request,
        CancellationToken cancellationToken = default)
    {
        LogGettingAllApplications();
        var applicationEntities = await repository.GetList(x => true, cancellationToken);

        var applications = applicationEntities.Select(app => new Application()
        {
            Id = app.ClientId,
            Name = app.Name,
            TenantName = app.TenantName,
            Description = app.Description,
            Secret = app.Secret,
            Scopes = app.Scopes.Split(Separator, StringSplitOptions.RemoveEmptyEntries).ToList(),
            RedirectUris = app.RedirectUris.Split(Separator, StringSplitOptions.RemoveEmptyEntries).ToList(),
        }).ToList();

        return applications;
    }

    public async Task<Application> Get(
        GetApplicationById request,
        CancellationToken cancellationToken = default)
    {
        LogGettingApplicationById(request.Id);
        Expression<Func<Applications, bool>> filter = x => x.ClientId == request.Id;

        var application = await repository.GetSingle(filter, cancellationToken);

        if (application == null)
        {
            LogApplicationWithIdNotFound(request.Id);
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
            RedirectUris = application.RedirectUris.Split(Separator, StringSplitOptions.RemoveEmptyEntries).ToList(),
        };
    }

    public async Task<Application> Update(
        UpdateApplication application,
        CancellationToken cancellationToken = default)
    {
        LogUpdatingApplicationWithId(application.Id);
        var existingApplication = await repository.GetSingle(x => x.ClientId.Equals(application.Id), cancellationToken);

        if (existingApplication == null)
        {
            LogApplicationWithIdNotFoundForUpdate(application.Id);
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
            RedirectUris = updated.RedirectUris.Split(Separator, StringSplitOptions.RemoveEmptyEntries).ToList(),
        };
    }

    public async Task<Application> Create(
        CreateApplication application,
        CancellationToken cancellationToken = default)
    {
        LogCreatingNewApplicationWithName(application.Name);

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
            RedirectUris = createdApplication.RedirectUris.Split(Separator, StringSplitOptions.RemoveEmptyEntries).ToList(),
        };
    }

    public async Task<bool> Delete(
        Guid id,
        Guid operatorId,
        CancellationToken cancellationToken = default)
    {
        LogDeletingApplicationWithIdByOperator(id, operatorId);
        return await repository.Delete(x => x.ClientId.Equals(id), operatorId, cancellationToken);
    }
}
