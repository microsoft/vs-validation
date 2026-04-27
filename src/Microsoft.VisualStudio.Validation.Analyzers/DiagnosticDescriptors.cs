// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Microsoft.VisualStudio.Validation.Analyzers;

/// <summary>
/// Defines the diagnostics supported by Microsoft.VisualStudio.Validation analyzers.
/// </summary>
public static class DiagnosticDescriptors
{
    /// <summary>
    /// Gets the diagnostic that offers <c>Requires.NotNull</c> for reference-type parameters.
    /// </summary>
    public static DiagnosticDescriptor AddRequiresNotNull { get; } = Create(
        DiagnosticIds.AddRequiresNotNull,
        "Add Requires.NotNull",
        "Add Requires.NotNull for parameter '{0}'");

    /// <summary>
    /// Gets the diagnostic that offers <c>Requires.Range</c> for supported numeric parameters.
    /// </summary>
    public static DiagnosticDescriptor AddRequiresRange { get; } = Create(
        DiagnosticIds.AddRequiresRange,
        "Add Requires.Range",
        "Add Requires.Range for parameter '{0}'");

    /// <summary>
    /// Gets the diagnostic that offers replacing a manual null check with <c>Requires.NotNull</c>.
    /// </summary>
    public static DiagnosticDescriptor UseRequiresNotNull { get; } = Create(
        DiagnosticIds.UseRequiresNotNull,
        "Use Requires.NotNull",
        "Replace the manual null check for '{0}' with Requires.NotNull");

    /// <summary>
    /// Gets the diagnostic that offers removing a redundant parameter-name argument from <c>Requires.NotNull</c>.
    /// </summary>
    public static DiagnosticDescriptor RemoveRedundantNotNullParameterName { get; } = Create(
        DiagnosticIds.RemoveRedundantNotNullParameterName,
        "Remove redundant Requires.NotNull argument name",
        "Remove the redundant parameter-name argument from Requires.NotNull");

    /// <summary>
    /// Gets all diagnostics supported by this analyzer package.
    /// </summary>
    public static ImmutableArray<DiagnosticDescriptor> All { get; } = ImmutableArray.Create(
        AddRequiresNotNull,
        AddRequiresRange,
        UseRequiresNotNull,
        RemoveRedundantNotNullParameterName);

    private static DiagnosticDescriptor Create(string id, string title, string messageFormat)
        => new(
            id,
            title,
            messageFormat,
            "Usage",
            DiagnosticSeverity.Hidden,
            isEnabledByDefault: true);
}
