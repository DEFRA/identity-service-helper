// <copyright file="IExternalMessagingRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Messaging;

using Defra.Identity.Postgres.Database.Entities;

public interface IExternalMessagingRepository :
    IRepository<ExternalMessaging>;
