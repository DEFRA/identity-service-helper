// <copyright file="Timestamps.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

using MongoDB.Bson.Serialization.Attributes;

namespace Defra.Identity.Mongo.Database.Documents;

public class Timestamps
{
    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
