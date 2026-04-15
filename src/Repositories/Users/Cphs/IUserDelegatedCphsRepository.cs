// <copyright file="IUserDelegatedCphsRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>
namespace Defra.Identity.Repositories.Users.Cphs;

using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common.Composites.Associations;

public interface IUserDelegatedCphsRepository : IListableAssociations<UserAccounts, CountyParishHoldingDelegations>;
