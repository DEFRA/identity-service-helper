// <copyright file="ErrorItem.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Responses.Messaging;

public class ErrorItem
{
    public string Error { get; set; }

    public string Message { get; set; }
}
