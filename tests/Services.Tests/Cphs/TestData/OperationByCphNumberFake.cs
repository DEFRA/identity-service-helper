// <copyright file="OperationByCphNumberFake.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Tests.Cphs.TestData;

using Defra.Identity.Models.Requests.Cphs.Common;

public class OperationByCphNumberFake : IOperationByCphNumber
{
    public OperationByCphNumberFake(int county, int parish, int holding)
    {
        County = county;
        Parish = parish;
        Holding = holding;
    }

    public int County { get; set; }

    public int Parish { get; set; }

    public int Holding { get; set; }
}
