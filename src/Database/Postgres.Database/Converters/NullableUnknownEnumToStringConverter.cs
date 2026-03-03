// <copyright file="NullableUnknownEnumToStringConverter.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Converters;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#pragma warning disable EF1001

/// <summary>
/// Converter to allow null values for an enum value.
/// </summary>
/// <typeparam name="TEnum">The enum type.</typeparam>
internal sealed class NullableUnknownEnumToStringConverter<TEnum>() : ValueConverter<TEnum?, string?>(
    enumValue => ToProviderValue(enumValue),
    stringValue => FromProviderValue(stringValue),
    convertsNulls: true)
    where TEnum : struct, Enum
{
    private static string? ToProviderValue(TEnum? enumValue)
    {
        if (!enumValue.HasValue || EqualityComparer<TEnum>.Default.Equals(enumValue.Value, default))
        {
            return null;
        }

        return enumValue.Value.ToString().ToUpperInvariant();
    }

    private static TEnum? FromProviderValue(string? stringValue)
    {
        return string.IsNullOrWhiteSpace(stringValue)
            ? default
            : Enum.Parse<TEnum>(stringValue, ignoreCase: true);
    }
}

#pragma warning restore EF1001
