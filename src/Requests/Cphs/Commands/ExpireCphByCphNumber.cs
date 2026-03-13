// <copyright file="ExpireCphByCphNumber.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Cphs.Commands;

using Defra.Identity.Requests.Cphs.Common;

public class ExpireCphByCphNumber : IOperationByCphNumber
{
    public int County { get; set; }

    public int Parish { get; set; }

    public int Holding { get; set; }
}
