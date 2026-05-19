// <copyright file="ApplicationService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Applications;

using Defra.Identity.Models.Requests.Applications.Commands;
using Defra.Identity.Models.Requests.Applications.Queries;
using Defra.Identity.Models.Responses.Applications;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Applications;
using Defra.Identity.Services.Common;
using Defra.Identity.Services.Common.Builders.Strategy.Factories;
using Defra.Identity.Services.Common.Context;
using Defra.Identity.Services.Common.Extensions;
using Defra.Identity.Services.Common.Filters;
using Defra.Identity.Services.Common.Mappers;
using FluentValidation;
using Microsoft.Extensions.Logging;

public class ApplicationService : IApplicationService
{
    private readonly IApplicationsRepository repository;
    private readonly IOperatorContext operatorContext;
    private readonly IStrategyBuilderFactory<ApplicationService> strategyBuilderFactory;
    private readonly IValidator<CreateApplication> createApplicationValidator;
    private readonly IValidator<UpdateApplicationByClientId> updateApplicationValidator;

    public ApplicationService(
        IApplicationsRepository repository,
        IOperatorContext operatorContext,
        IStrategyBuilderFactory<ApplicationService> strategyBuilderFactory,
        IValidator<CreateApplication> createApplicationValidator,
        IValidator<UpdateApplicationByClientId> updateApplicationValidator,
        ILogger<ApplicationService> logger)
    {
        this.repository = repository;
        this.operatorContext = operatorContext;
        this.strategyBuilderFactory = strategyBuilderFactory;
        this.createApplicationValidator = createApplicationValidator;
        this.updateApplicationValidator = updateApplicationValidator;

        this.strategyBuilderFactory
            .WithDefaultLogger(logger)
            .WithDefaultOperatorContext(operatorContext)
            .WithDefaultEntityDescription(EntityDescriptions.Application);
    }

    public async Task<List<Application>> GetAll(
        GetApplications request,
        CancellationToken cancellationToken = default)
    {
        return await strategyBuilderFactory.BuildGetListStrategy<Applications>()
            .WithActionDescription("Get all applications")
            .WithRepository(repository)
            .WithCancellationToken(cancellationToken)
            .WithEntityFilter(FilterLibrary.Applications.NotSoftDeleted)
            .ExecuteAndMap(ApplicationMapper.MapApplicationEntityToApplication);
    }

    public async Task<Application> Get(
        GetApplicationByClientId request,
        CancellationToken cancellationToken = default)
    {
        var applicationFilter = FilterLibrary.Applications.NotSoftDeleted
            .AndAlso(application => request.Id == application.ClientId);

        return await strategyBuilderFactory.BuildGetStrategy<Applications>()
            .WithActionDescription("Get application")
            .WithRepository(repository)
            .WithCancellationToken(cancellationToken)
            .WithRequest(request)
            .WithEntityFilter(applicationFilter)
            .ExecuteAndMap(ApplicationMapper.MapApplicationEntityToApplication);
    }

    public async Task<Application> Create(
        CreateApplication request,
        CancellationToken cancellationToken = default)
    {
        return await strategyBuilderFactory.BuildCreateStrategy<Applications>()
            .WithActionDescription("Create application")
            .WithRepository(repository)
            .WithCancellationToken(cancellationToken)
            .WithRequestValidation(() => createApplicationValidator.ValidateAsync(request, cancellationToken))
            .WithCreate(
                () => new Applications
                {
                    Name = request.Name,
                    ClientId = request.Id,
                    TenantName = request.TenantName,
                    Description = request.Description,
                    CreatedById = operatorContext.OperatorId,
                    Scopes = string.Join(ApplicationMapper.ScopeSeparator, request.Scopes),
                    Secret = request.Secret,
                    RedirectUris = string.Join(ApplicationMapper.RedirectUriSeparator, request.RedirectUris),
                })
            .ExecuteAndMap(ApplicationMapper.MapApplicationEntityToApplication);
    }

    public async Task<Application> Update(
        UpdateApplicationByClientId request,
        CancellationToken cancellationToken = default)
    {
        var applicationFilter = FilterLibrary.Applications.NotSoftDeleted
            .AndAlso(application => request.Id == application.ClientId);

        return await strategyBuilderFactory.BuildUpdateStrategy<Applications>()
            .WithActionDescription("Update application")
            .WithRepository(repository)
            .WithCancellationToken(cancellationToken)
            .WithRequestValidation(() => updateApplicationValidator.ValidateAsync(request, cancellationToken))
            .WithRequest(request)
            .WithEntityFilter(applicationFilter)
            .WithUpdate(
                application =>
                {
                    application.Name = request.Name;
                    application.TenantName = request.TenantName;
                    application.Description = request.Description;
                    application.Scopes = string.Join(ApplicationMapper.ScopeSeparator, request.Scopes);
                    application.RedirectUris = string.Join(ApplicationMapper.RedirectUriSeparator, request.RedirectUris);
                    application.Secret = request.Secret;
                })
            .ExecuteAndMap(ApplicationMapper.MapApplicationEntityToApplication);
    }

    public async Task Delete(DeleteApplicationByClientId request, CancellationToken cancellationToken = default)
    {
        var applicationFilter = FilterLibrary.Applications.NotSoftDeleted
            .AndAlso(application => request.Id == application.ClientId);

        await strategyBuilderFactory.BuildUpdateStrategy<Applications>()
            .WithActionDescription("Delete application")
            .WithRepository(repository)
            .WithCancellationToken(cancellationToken)
            .WithRequest(request)
            .WithEntityFilter(applicationFilter)
            .WithUpdate(
                application =>
                {
                    application.DeletedAt = DateTime.UtcNow;
                    application.DeletedById = operatorContext.OperatorId;
                })
            .Execute();
    }
}
