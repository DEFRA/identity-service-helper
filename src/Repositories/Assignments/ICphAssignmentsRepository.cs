// <copyright file="ICphAssignmentsRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Assignments;

using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common.Composites.Associations;

public interface ICphAssignmentsRepository : IPageableAssociations<CountyParishHoldings, ApplicationUserAccountHoldingAssignments>;
