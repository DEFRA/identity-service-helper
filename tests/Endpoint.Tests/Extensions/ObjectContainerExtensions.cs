// <copyright file="ObjectContainerExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Reqnroll.BoDi;

using System.Linq;
using System.Reflection;
using Microsoft;

/// <summary>
/// Collection of functions that extend the Reqnroll object collection.
/// </summary>
public static class ObjectContainerExtensions
{
    /// <summary>
    /// Extension to add all classes that implement the interface T into the service collection.
    /// </summary>
    /// <param name="source">The service collection to popluate.</param>
    /// <typeparam name="T">The interface to find.</typeparam>
    public static void RegisterAllImplementations<T>(this ObjectContainer source)
    {
        Requires.NotNull(source);

        var interfaceType = typeof(T);
        var implementations = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => interfaceType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);

        foreach (var implementation in implementations)
        {
            source.RegisterTypeAs(implementation, interfaceType);
        }
    }
}
