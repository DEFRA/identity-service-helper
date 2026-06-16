// <copyright file="CphNumberService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Cphs;

using System.Linq.Expressions;
using Defra.Identity.Models.Requests.Cphs.Common;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common.Exceptions;
using Defra.Identity.Repositories.Cphs;
using FluentValidation;
using Microsoft.Extensions.Logging;

public partial class CphNumberService(
    ICphRepository cphRepository,
    IValidator<IOperationByCphNumber> cphNumberValidator,
    ILogger<CphNumberService> logger)
    : ICphNumberService
{
    public async Task<Guid> GetIdFromCphNumber(IOperationByCphNumber request, CancellationToken cancellationToken = default)
    {
        var cphNumberValidationResult = await cphNumberValidator.ValidateAsync(request, cancellationToken);

        if (!cphNumberValidationResult.IsValid)
        {
            throw new ValidationException(cphNumberValidationResult.Errors);
        }

        var formattedCphNumber = $"{request.County:D2}/{request.Parish:D3}/{request.Holding:D4}";

        LogGettingCountyParishHoldingIdByCphNumber(formattedCphNumber);

        Expression<Func<CountyParishHoldings, bool>> filter = cph => cph.Identifier == formattedCphNumber;

        var cphEntity = await cphRepository.GetSingle(filter, cancellationToken);

        if (cphEntity is not { DeletedAt: null })
        {
            LogCountyParishHoldingWithCphNumberNotFound(formattedCphNumber);

            throw new NotFoundException("County parish holding not found");
        }

        return cphEntity.Id;
    }
}
