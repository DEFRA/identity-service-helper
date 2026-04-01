// <copyright file="GetCphUsersByCphNumber.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Cphs.Queries;

using Defra.Identity.Requests.Common.Queries;
using Defra.Identity.Requests.Cphs.Common;

public class GetCphUsersByCphNumber : PagedQuery, IOperationByCphNumber
{
    public int County { get; set; }

    public int Parish { get; set; }

    public int Holding { get; set; }
}
