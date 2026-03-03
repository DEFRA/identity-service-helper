// <copyright file="RequiredUnknownEnumToStringConverter.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Converters;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#pragma warning disable EF1001

/// <summary>
/// Converter to Require that an enum value is not Unknown (0).
/// </summary>
/// <typeparam name="TEnum">The enum type.</typeparam>
internal sealed class RequiredUnknownEnumToStringConverter<TEnum>() : ValueConverter<TEnum, string>(
    enumValue => ToProviderValue(enumValue),
    stringValue => FromProviderValue(stringValue),
    convertsNulls: true)
    where TEnum : struct, Enum
{
    private static string ToProviderValue(TEnum enumValue)
    {
        return EqualityComparer<TEnum>.Default.Equals(enumValue, default)
            ? throw new InvalidOperationException(
                $"{typeof(TEnum).Name}.Unknown (0) is not valid for non-nullable enum properties.")
            : enumValue.ToString().ToUpperInvariant();
    }

    private static TEnum FromProviderValue(string stringValue)
    {
        return string.IsNullOrWhiteSpace(stringValue)
            ? throw new InvalidOperationException(
                $"{typeof(TEnum).Name} value cannot be null/empty for non-nullable enum properties.")
            : Enum.Parse<TEnum>(stringValue, ignoreCase: true);
    }
}

#pragma warning restore EF1001
