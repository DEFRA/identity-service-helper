// <copyright file="StringProcessor.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Endpoint.Tests.Support;

using System.Text.RegularExpressions;
using Reqnroll;

public class StringProcessor(
    FeatureContext featureContext)
{
    private static readonly Regex ContextRegex = new Regex(@"\<(?'variable'.+)\>", RegexOptions.IgnoreCase);

    public string ProcessString(string token)
    {
        string result = token;
        result = ContextRegex.IsMatch(result) ? this.HandleContextLookup(result) : result;
        return result;
    }

    private string HandleContextLookup(string value)
    {
        string variableToFind = ContextRegex.Match(value).Groups["variable"].Value;
        return featureContext.TryGetValue(variableToFind, out string variableValue)
            ? ContextRegex.Replace(value, variableValue)
            : value;
    }
}
