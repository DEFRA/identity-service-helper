// <copyright file="KrdsProvider.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Providers;

using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Web;
using Defra.Identity.Models.Integration.Krds.Parties;
using Defra.Identity.Models.Integration.Krds.Sites;
using Json.Schema;
using Microsoft.Extensions.Logging;

public class KrdsProvider(HttpClient client, ILogger<KrdsProvider> logger) : IKrdsProvider
{
    public async Task<SiteResponse> Sites(DateTime since, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting sites");
        var request = string.Concat(client.BaseAddress, GetSitesSince(since));

        logger.LogInformation("Request: {Request}", request);
        using var response = await client.GetAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync(cancellationToken);

        if (string.IsNullOrEmpty(result) || !ValidateSitesJson(result))
        {
            return new SiteResponse();
        }

        try
        {
            if (!result.TrimStart().StartsWith('['))
            {
                return JsonSerializer.Deserialize<SiteResponse>(result)!;
            }

            var sites = JsonSerializer.Deserialize<List<Models.Integration.Krds.Sites.Site>>(result);
            return new SiteResponse
            {
                Values = sites ?? [],
                Count = sites?.Count ?? 0,
            };
        }
        catch (JsonException jsonException)
        {
            logger.LogError(jsonException, "Error deserializing sites. Body: {ResponseBody}", result);
            throw;
        }
    }

    public async Task<PartyResponse> Parties(DateTime since, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting parties");
        var request = string.Concat(client.BaseAddress, GetPartiesSince(since));

        logger.LogInformation("Request: {Request}", request);
        using var response = await client.GetAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync(cancellationToken);

        if (string.IsNullOrEmpty(result) || !ValidatePartiesJson(result))
        {
            return new PartyResponse();
        }

        try
        {
            if (result.TrimStart().StartsWith('['))
            {
                var parties = JsonSerializer.Deserialize<List<Models.Integration.Krds.Parties.Party>>(result);
                return new PartyResponse
                {
                    Values = parties ?? [],
                    Count = parties?.Count ?? 0,
                };
            }

            return JsonSerializer.Deserialize<PartyResponse>(result)!;
        }
        catch (JsonException jsonException)
        {
            logger.LogError(jsonException, "Error deserializing parties. Body: {ResponseBody}", result);
            throw;
        }
    }

    private static string GetSitesSince(DateTime since) => $"sites?since={HttpUtility.UrlEncode(since.ToString("yyyy-MM-dd"))}";

    private static string GetPartiesSince(DateTime since) => $"parties?since={HttpUtility.UrlEncode(since.ToString("yyyy-MM-dd"))}";

    private static JsonSchema LoadSchemaFromResource()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "Defra.Identity.KeeperReferenceData.Json.sites_response.schema.json";

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            throw new InvalidOperationException($"Could not find embedded resource: {resourceName}");
        }

        using var reader = new StreamReader(stream);
        var schemaJson = reader.ReadToEnd();
        return JsonSchema.FromText(schemaJson);
    }

    private static JsonSchema LoadPartiesSchemaFromResource()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "Defra.Identity.KeeperReferenceData.Json.parties_response.schema.json";

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            throw new InvalidOperationException($"Could not find embedded resource: {resourceName}");
        }

        using var reader = new StreamReader(stream);
        var schemaJson = reader.ReadToEnd();
        return JsonSchema.FromText(schemaJson);
    }

    private bool ValidateSitesJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return true;
        }

        if (json.TrimStart().StartsWith('['))
        {
            return true;
        }

        var schema = LoadSchemaFromResource();
        var jsonNode = JsonNode.Parse(json);
        var validationResults = schema.Evaluate(jsonNode);

        if (!validationResults.IsValid)
        {
            logger.LogError("JSON Schema validation failed: {Errors}", string.Join(", ", validationResults.Errors?.Select(x => $"{x.Key}: {x.Value}") ?? []));
            return false;
        }

        logger.LogInformation("Sites JSON schema validation successful");
        return true;
    }

    private bool ValidatePartiesJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return true;
        }

        if (json.TrimStart().StartsWith('['))
        {
            return true;
        }

        var schema = LoadPartiesSchemaFromResource();
        var jsonNode = JsonNode.Parse(json);
        var validationResults = schema.Evaluate(jsonNode);

        if (!validationResults.IsValid)
        {
            logger.LogError("JSON Schema validation failed: {Errors}", string.Join(", ", validationResults.Errors?.Select(x => $"{x.Key}: {x.Value}") ?? []));
            return false;
        }

        logger.LogInformation("Parties JSON schema validation successful");
        return true;
    }

    public void Dispose()
    {
        client.Dispose();
    }
}
