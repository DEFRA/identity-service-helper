// <copyright file="OperationByCphNumber.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Cphs.Common;

using System.ComponentModel;

public class OperationByCphNumber : IOperationByCphNumber
{
    [Description(OpenApiMetadata.CountyElement)]
    public int County { get; set; }

    [Description(OpenApiMetadata.ParishElement)]
    public int Parish { get; set; }

    [Description(OpenApiMetadata.HoldingElement)]
    public int Holding { get; set; }
}
