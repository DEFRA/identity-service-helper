// <copyright file="GetCphAssigneesByCphNumber.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Cphs.Queries;

using System.ComponentModel;
using Defra.Identity.Models.Requests.Common.Queries;
using Defra.Identity.Models.Requests.Cphs.Common;

public class GetCphAssigneesByCphNumber : PagedQuery, IOperationByCphNumber
{
    [Description(OpenApiMetadata.CountyElement)]
    public int County { get; set; }

    [Description(OpenApiMetadata.CountyElement)]
    public int Parish { get; set; }

    [Description(OpenApiMetadata.CountyElement)]
    public int Holding { get; set; }
}
