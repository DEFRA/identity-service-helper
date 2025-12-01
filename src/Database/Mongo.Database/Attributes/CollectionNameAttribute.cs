// <copyright file="CollectionName.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Mongo.Database.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class CollectionNameAttribute(string name) : Attribute
{
    public string Name { get;  } = name;
}
