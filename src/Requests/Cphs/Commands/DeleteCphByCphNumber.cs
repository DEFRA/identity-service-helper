// <copyright file="DeleteCphByCphNumber.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Cphs.Commands;

public class DeleteCphByCphNumber : IOperationByCphNumber
{
    public int County { get; set; }

    public int Parish { get; set; }

    public int Holding { get; set; }
}
