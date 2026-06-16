// <copyright file="ServiceFake.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Tests.Common.TestData;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class ServiceFake
{
    public string Name { get; set; } = string.Empty;
}
