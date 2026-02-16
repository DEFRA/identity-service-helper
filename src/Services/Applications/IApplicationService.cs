// <copyright file="IApplicationService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Applications;

using Defra.Identity.Requests.Applications.Commands.Create;
using Defra.Identity.Requests.Applications.Commands.Update;
using Defra.Identity.Requests.Applications.Queries;
using Defra.Identity.Responses.Applications;

public interface IApplicationService
{
    Task<List<Application>> GetAll(GetApplications request, CancellationToken cancellationToken = default);

    Task<Application> Get(GetApplicationById request, CancellationToken cancellationToken = default);

    Task<Application> Update(UpdateApplication application, CancellationToken cancellationToken = default);

    Task<Application> Create(CreateApplication application, CancellationToken cancellationToken = default);

    Task<bool> Delete(Guid id, Guid operatorId, CancellationToken cancellationToken = default);
}
