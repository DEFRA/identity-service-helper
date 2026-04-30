// <copyright file="MessageTypes.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database;

public enum MessageTypes
{
    /// <summary>
    /// An email message.
    /// </summary>
    Email = 1,

    /// <summary>
    /// An SMS message.
    /// </summary>
    Sms = 2,
}
