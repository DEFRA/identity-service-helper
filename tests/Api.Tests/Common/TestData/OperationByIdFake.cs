// <copyright file="OperationByIdFake.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Tests.Common.TestData;

using System.Diagnostics.CodeAnalysis;
using Defra.Identity.Models.Requests.Common;

[ExcludeFromCodeCoverage]
public class OperationByIdFake : OperationById<Guid>;
