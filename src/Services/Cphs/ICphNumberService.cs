// <copyright file="ICphNumberService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Cphs;

using Defra.Identity.Models.Requests.Cphs.Common;

public interface ICphNumberService
{
    Task<Guid> GetIdFromCphNumber(IOperationByCphNumber request, CancellationToken cancellationToken = default);
}
