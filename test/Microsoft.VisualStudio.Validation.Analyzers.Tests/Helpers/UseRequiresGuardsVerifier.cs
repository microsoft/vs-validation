// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.Validation.Analyzers;

namespace Microsoft.VisualStudio.Validation.Analyzers.Tests.Helpers;

internal static class UseRequiresGuardsVerifier
{
    internal static DiagnosticResult Diagnostic()
        => CSharpCodeFixVerifier<CSharpUseRequiresGuardsAnalyzer, CSharpUseRequiresGuardsCodeFixProvider>.Diagnostic();

    internal static DiagnosticResult Diagnostic(string diagnosticId)
        => CSharpCodeFixVerifier<CSharpUseRequiresGuardsAnalyzer, CSharpUseRequiresGuardsCodeFixProvider>.Diagnostic(diagnosticId);

    internal static Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
        => CSharpCodeFixVerifier<CSharpUseRequiresGuardsAnalyzer, CSharpUseRequiresGuardsCodeFixProvider>.VerifyAnalyzerAsync(source, expected);

    internal static Task VerifyCodeFixAsync(string source, string fixedSource)
        => CSharpCodeFixVerifier<CSharpUseRequiresGuardsAnalyzer, CSharpUseRequiresGuardsCodeFixProvider>.VerifyCodeFixAsync(source, fixedSource);
}
