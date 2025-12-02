// <copyright file="BaseAudit.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Mongo.Database.Documents;

using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

public abstract class BaseAudit
{
    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}
