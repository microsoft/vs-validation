// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.Validation.Analyzers.Tests.Helpers;
using Xunit;

namespace Microsoft.VisualStudio.Validation.Analyzers.Tests;

public class UseRequiresGuardsAnalyzerTests
{
    [Fact]
    public async Task ReferenceTypeParameter_WithBody_ProducesDiagnostic()
    {
        string test = """
            class Test
            {
                void M(string {|VSV0001:value|})
                {
                }
            }
            """;

        await UseRequiresGuardsVerifier.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task ReferenceTypeParameter_WithExistingGuard_ProducesNoDiagnostic()
    {
        string test = """
            using Microsoft;

            class Test
            {
                void M(string value)
                {
                    Requires.NotNull(value);
                }
            }
            """;

        await UseRequiresGuardsVerifier.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task NumericParameter_WithBody_ProducesDiagnostic()
    {
        string test = """
            class Test
            {
                void M(int {|VSV0002:count|})
                {
                }
            }
            """;

        await UseRequiresGuardsVerifier.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task UnsignedNumericParameter_ProducesNoDiagnostic()
    {
        string test = """
            class Test
            {
                void M(uint count)
                {
                }
            }
            """;

        await UseRequiresGuardsVerifier.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task NullableNumericParameter_ProducesNoDiagnostic()
    {
        string test = """
            class Test
            {
                void M(int? count)
                {
                }
            }
            """;

        await UseRequiresGuardsVerifier.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task NumericParameter_WithMismatchedRequiresRangeGuard_ProducesDiagnostic()
    {
        string test = """
            using Microsoft;

            class Test
            {
                void M(int {|VSV0002:count|}, int {|VSV0002:other|})
                {
                    Requires.Range(other >= 0, nameof(count));
                }
            }
            """;

        await UseRequiresGuardsVerifier.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task ExpressionBodiedMember_ProducesNoDiagnostic()
    {
        string test = """
            class Test
            {
                void M(string value) => System.Console.WriteLine(value);
            }
            """;

        await UseRequiresGuardsVerifier.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task ManualNullCheck_WithArgumentNullException_ProducesDiagnostic()
    {
        string test = """
            class Test
            {
                void M(string value)
                {
                    {|VSV0003:if (value is null)
                    {
                        throw new System.ArgumentNullException(nameof(value));
                    }|}
                }
            }
            """;

        await UseRequiresGuardsVerifier.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task ManualNullCheck_WithElse_ProducesRequiresNotNullDiagnostic()
    {
        string test = """
            class Test
            {
                void M(string {|VSV0001:value|})
                {
                    if (value is null)
                    {
                        throw new System.ArgumentNullException(nameof(value));
                    }
                    else
                    {
                        System.Console.WriteLine(value);
                    }
                }
            }
            """;

        await UseRequiresGuardsVerifier.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task ManualNullCheck_WithDifferentException_ProducesRequiresNotNullDiagnostic()
    {
        string test = """
            class Test
            {
                void M(string {|VSV0001:value|})
                {
                    if (value is null)
                    {
                        throw new System.ArgumentException(nameof(value));
                    }
                }
            }
            """;

        await UseRequiresGuardsVerifier.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task ReferenceTypeParameter_WithNonThrowingNullCheck_ProducesDiagnostic()
    {
        string test = """
            class Test
            {
                void M(string {|VSV0001:value|})
                {
                    if (value is null)
                    {
                        return;
                    }
                }
            }
            """;

        await UseRequiresGuardsVerifier.VerifyAnalyzerAsync(test);
    }
}
