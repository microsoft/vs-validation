// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace Microsoft;

/// <summary>
/// Indicates to Code Analysis that a method validates a particular parameter.
/// </summary>
[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
public sealed class ValidatedNotNullAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidatedNotNullAttribute"/> class.
    /// </summary>
    public ValidatedNotNullAttribute()
    {
    }
}
