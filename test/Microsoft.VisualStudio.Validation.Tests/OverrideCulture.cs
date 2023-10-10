// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;

/// <summary>
/// Sets a specific culture for a duration, restoring the prior culture upon disposal.
/// </summary>
internal sealed class OverrideCulture : IDisposable
{
    private readonly CultureInfo priorCulture;
    private readonly CultureInfo priorUICulture;

    public OverrideCulture(CultureInfo culture)
    {
        this.priorCulture = CultureInfo.CurrentCulture;
        this.priorUICulture = CultureInfo.CurrentUICulture;

        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
    }

    public void Dispose()
    {
        CultureInfo.CurrentCulture = this.priorCulture;
        CultureInfo.CurrentUICulture = this.priorUICulture;
    }
}
