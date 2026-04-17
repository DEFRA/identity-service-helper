// <copyright file="SchemaTransformer.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Utility.OpenApi;

using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

public class SchemaTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(
        OpenApiSchema schema,
        OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken)
    {
        Debug.WriteLine(context.JsonPropertyInfo?.Name);
        if (!string.IsNullOrWhiteSpace(schema.Description))
        {
            return Task.CompletedTask;
        }

        string? description = null;

        // DTO property description: [Description("...")]
        if (context.JsonPropertyInfo?.AttributeProvider is MemberInfo memberInfo)
        {
            description = memberInfo.GetCustomAttribute<DescriptionAttribute>()?.Description;
        }

        // DTO type description: [Description("...")] on class
        if (description is null)
        {
            description = context.JsonTypeInfo.Type
                .GetCustomAttribute<DescriptionAttribute>()?.Description;
        }

        // Endpoint parameter metadata (when available)
        if (description is null)
        {
            description = context.ParameterDescription?.ModelMetadata?.Description;
        }

        if (!string.IsNullOrWhiteSpace(description))
        {
            schema.Description = description;
        }

        return Task.CompletedTask;
    }
}
