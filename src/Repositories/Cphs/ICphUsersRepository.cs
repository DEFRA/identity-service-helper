// <copyright file="ICphUsersRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Cphs;

using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common.Composites.Associations;

public interface ICphUsersRepository : IPageableAssociations<CountyParishHoldings, ApplicationUserAccountHoldingAssignments>;
