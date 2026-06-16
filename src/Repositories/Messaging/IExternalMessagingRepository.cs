// <copyright file="IExternalMessagingRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Messaging;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common.Composites;

public interface IExternalMessagingRepository :
    IGettable<ExternalMessaging>, IListable<ExternalMessaging>, ICreatable<ExternalMessaging>,
    IUpdatable<ExternalMessaging>;
