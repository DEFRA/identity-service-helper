// <copyright file="ICphDelegationsForDelegateRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>
namespace Defra.Identity.Repositories.Delegations;

using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common.Composites.Associations;

public interface ICphDelegationsForDelegateRepository : IListableAssociations<UserAccounts, CountyParishHoldingDelegations>;
