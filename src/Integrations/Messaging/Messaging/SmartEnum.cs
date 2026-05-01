// <copyright file="SmartEnum.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models;

using System.Reflection;

public abstract class SmartEnum<TEnum, TValueType>(string name, TValueType value)
    where TEnum : SmartEnum<TEnum,TValueType>
{
    private static readonly Lazy<IReadOnlyCollection<TEnum>> AllItems = new(() =>
        typeof(TEnum)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Where(field => field.FieldType == typeof(TEnum))
            .Select(field => (TEnum)field.GetValue(null)!)
            .ToArray());

    public static IReadOnlyCollection<TEnum> List => AllItems.Value;

    public string Name { get; } = name ?? throw new ArgumentNullException(nameof(name));

    public TValueType Value { get; } = value;

    public static TEnum FromName(string name) =>
        List.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.Ordinal))
        ?? throw new ArgumentException($"Unknown {typeof(TEnum).Name} name '{name}'.", nameof(name));

    public static TEnum FromValue(string value) =>
        List.FirstOrDefault(x => string.Equals(x.Value!.ToString(), value, StringComparison.Ordinal))
        ?? throw new ArgumentException($"Unknown {typeof(TEnum).Name} value '{value}'.", nameof(value));

    public override string ToString() => Value!.ToString()!;
}
