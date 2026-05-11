// <copyright file="IOperationByCphNumber.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Cphs.Common;

public interface IOperationByCphNumber
{
    int County { get; set; }

    int Parish { get; set; }

    int Holding { get; set; }
}
