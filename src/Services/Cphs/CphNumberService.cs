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

public class CphNumberService : ICphNumberService
{
    private readonly ICphRepository cphRepository;
    private readonly IValidator<IOperationByCphNumber> cphNumberValidator;
    private readonly ILogger<CphNumberService> logger;

    public CphNumberService(ICphRepository cphRepository, IValidator<IOperationByCphNumber> cphNumberValidator, ILogger<CphNumberService> logger)
    {
        this.cphRepository = cphRepository;
        this.cphNumberValidator = cphNumberValidator;
        this.logger = logger;
    }

    public async Task<Guid> GetIdFromCphNumber(IOperationByCphNumber request, CancellationToken cancellationToken = default)
    {
        var cphNumberValidationResult = await cphNumberValidator.ValidateAsync(request, cancellationToken);

        if (!cphNumberValidationResult.IsValid)
        {
            throw new ValidationException(cphNumberValidationResult.Errors);
        }

        var formattedCphNumber = $"{request.County:D2}/{request.Parish:D3}/{request.Holding:D4}";

        logger.LogInformation("Getting county parish holding id by cph number {FormattedCphNumber}", formattedCphNumber);

        Expression<Func<CountyParishHoldings, bool>> filter = cph => cph.Identifier == formattedCphNumber;

        var cphEntity = await cphRepository.GetSingle(filter, cancellationToken);

        if (cphEntity is not { DeletedAt: null })
        {
            logger.LogWarning("County parish holding with cph number {FormattedCphNumber} not found", formattedCphNumber);

            throw new NotFoundException("County parish holding not found.");
        }

        return cphEntity.Id;
    }
}
