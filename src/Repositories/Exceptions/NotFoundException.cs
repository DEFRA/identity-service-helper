// <copyright file="NotFoundException.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Exceptions;

public sealed class NotFoundException : Exception
{
    public NotFoundException(string message)
        : base(message)
    {
    }
}
