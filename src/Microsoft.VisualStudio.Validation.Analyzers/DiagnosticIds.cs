// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.VisualStudio.Validation.Analyzers;

/// <summary>
/// Defines diagnostic identifiers for Microsoft.VisualStudio.Validation analyzers.
/// </summary>
public static class DiagnosticIds
{
    /// <summary>
    /// The diagnostic ID for offering <c>Requires.NotNull</c> on reference-type parameters.
    /// </summary>
    public const string AddRequiresNotNull = "VSV0001";

    /// <summary>
    /// The diagnostic ID for offering <c>Requires.Range</c> on supported numeric parameters.
    /// </summary>
    public const string AddRequiresRange = "VSV0002";

    /// <summary>
    /// The diagnostic ID for replacing a manual null check with <c>Requires.NotNull</c>.
    /// </summary>
    public const string UseRequiresNotNull = "VSV0003";

    /// <summary>
    /// The diagnostic ID for removing redundant parameter-name arguments from <c>Requires.NotNull</c>.
    /// </summary>
    public const string RemoveRedundantNotNullParameterName = "VSV0004";
}
