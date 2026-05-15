// <copyright file="ILoggableById.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Common;

public interface ILoggableById
{
    string GetLoggableId();
}
