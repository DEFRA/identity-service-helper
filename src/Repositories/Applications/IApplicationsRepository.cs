// <copyright file="IApplicationsRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Applications;

public interface IApplicationsRepository :
    IGetListRepository<Postgres.Database.Entities.Applications>,
    IGetSingleRepository<Postgres.Database.Entities.Applications>,
    ICreateRepository<Postgres.Database.Entities.Applications>,
    IUpdateRepository<Postgres.Database.Entities.Applications>,
    IDeleteRepository<Postgres.Database.Entities.Applications>;
