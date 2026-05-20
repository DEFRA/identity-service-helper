// <copyright file="IApplicationService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Applications;

using Defra.Identity.Models.Requests.Applications.Commands;
using Defra.Identity.Models.Requests.Applications.Queries;
using Defra.Identity.Models.Responses.Applications;

public interface IApplicationService
{
    Task<List<Application>> GetAll(GetApplications request, CancellationToken cancellationToken = default);

    Task<Application> Get(GetApplicationByClientId request, CancellationToken cancellationToken = default);

    Task<Application> Create(CreateApplication request, CancellationToken cancellationToken = default);

    Task<Application> Update(UpdateApplicationByClientId request, CancellationToken cancellationToken = default);

    Task Delete(DeleteApplicationByClientId request, CancellationToken cancellationToken = default);
}
