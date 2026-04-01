// <copyright file="HttpRequestHeadersExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace System.Net.Http.Headers;

public static class HttpRequestHeadersExtensions
{
    /// <summary>
    /// Add or replace a header value.
    /// </summary>
    /// <param name="source">The Request headers to work with.</param>
    /// <param name="key">The key to add or replace.</param>
    /// <param name="value">The value to add or replace.</param>
    public static void AddOrReplace(this HttpRequestHeaders source, string key, string value)
    {
        if (source.Contains(key))
        {
            source.Remove(key);
        }

        source.Add(key, value);
    }
}
