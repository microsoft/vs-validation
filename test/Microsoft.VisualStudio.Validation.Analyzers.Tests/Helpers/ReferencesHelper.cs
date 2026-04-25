// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis.Testing;

namespace Microsoft.VisualStudio.Validation.Analyzers.Tests.Helpers;

internal static class ReferencesHelper
{
    internal static readonly ReferenceAssemblies References = CreateReferenceAssemblies();

    private static ReferenceAssemblies CreateReferenceAssemblies()
        => ReferenceAssemblies.Net.Net80.WithNuGetConfigFilePath(FindNuGetConfigPath());

    private static string FindNuGetConfigPath()
    {
        string? path = AppContext.BaseDirectory;
        while (path is not null)
        {
            string candidate = Path.Combine(path, "nuget.config");
            if (File.Exists(candidate))
            {
                return candidate;
            }

            path = Path.GetDirectoryName(path);
        }

        throw new InvalidOperationException("Could not find nuget.config by searching up from " + AppContext.BaseDirectory);
    }
}
