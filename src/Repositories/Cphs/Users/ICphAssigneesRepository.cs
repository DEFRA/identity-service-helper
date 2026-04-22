// <copyright file="ICphAssigneesRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Cphs.Users;

using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common.Composites.Associations;

public interface ICphAssigneesRepository : IPageableAssociations<CountyParishHoldings, ApplicationUserAccountHoldingAssignments>;
