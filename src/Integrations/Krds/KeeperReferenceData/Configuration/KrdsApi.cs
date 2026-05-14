// <copyright file="KrdsApi.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Configuration;

public class KrdsApi
{
    public string Url { get; set; }

    public string ClientId { get; set; }

    public string ClientSecret { get; set; }

    public string TokenUrl { get; set; }
}
