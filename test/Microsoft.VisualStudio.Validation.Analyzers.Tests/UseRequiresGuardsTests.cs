// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.VisualStudio.Validation.Analyzers.Tests.Helpers;
using Xunit;

namespace Microsoft.VisualStudio.Validation.Analyzers.Tests;

public class UseRequiresGuardsTests
{
    [Fact]
    public async Task ReferenceTypeParameter_CodeFixAddsRequiresNotNull()
    {
        string test = """
            class Test
            {
                void M(string {|VSV0001:value|})
                {
                    System.Console.WriteLine(value);
                }
            }
            """;

        string fixedCode = """
            using Microsoft;

            class Test
            {
                void M(string value)
                {
                    Requires.NotNull(value);
                    System.Console.WriteLine(value);
                }
            }
            """;

        await UseRequiresGuardsVerifier.VerifyCodeFixAsync(test, fixedCode);
    }

    [Fact]
    public async Task ReferenceTypeParameter_WithEscapedIdentifier_PreservesIdentifierText()
    {
        string test = """
            class Test
            {
                void M(string {|VSV0001:@class|})
                {
                    System.Console.WriteLine(@class);
                }
            }
            """;

        string fixedCode = """
            using Microsoft;

            class Test
            {
                void M(string @class)
                {
                    Requires.NotNull(@class);
                    System.Console.WriteLine(@class);
                }
            }
            """;

        await UseRequiresGuardsVerifier.VerifyCodeFixAsync(test, fixedCode);
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
    public async Task RequiresNotNull_WithRedundantStringLiteralParameterName_CodeFixRemovesArgument()
    {
        string test = """
            using Microsoft;

            class Test
            {
                void M(string value)
                {
                    Requires.NotNull(value, {|VSV0004:"value"|});
                }
            }
            """;

        string fixedCode = """
            using Microsoft;

            class Test
            {
                void M(string value)
                {
                    Requires.NotNull(value);
                }
            }
            """;

        await UseRequiresGuardsVerifier.VerifyCodeFixAsync(test, fixedCode);
    }

    [Fact]
    public async Task RequiresNotNull_WithRedundantNameofParameterName_CodeFixRemovesArgument()
    {
        string test = """
            using Microsoft;

            class Test
            {
                void M(string value)
                {
                    Requires.NotNull(value, {|VSV0004:nameof(value)|});
                }
            }
            """;

        string fixedCode = """
            using Microsoft;

            class Test
            {
                void M(string value)
                {
                    Requires.NotNull(value);
                }
            }
            """;

        await UseRequiresGuardsVerifier.VerifyCodeFixAsync(test, fixedCode);
    }

    [Fact]
    public async Task RequiresNotNull_WithEscapedIdentifierParameterName_ProducesNoDiagnostic()
    {
        string test = """
            using Microsoft;

            class Test
            {
                void M(string @class)
                {
                    Requires.NotNull(@class, nameof(@class));
                }
            }
            """;

        await UseRequiresGuardsVerifier.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task ReferenceTypeParameter_WithNullableAnnotationsDisabled_ProducesDiagnostic()
    {
        string test = """
            #nullable disable
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
    public async Task NullableReferenceTypeParameter_ProducesNoDiagnostic()
    {
        string test = """
            #nullable enable
            class Test
            {
                void M(string? value)
                {
                }
            }
            """;

        await UseRequiresGuardsVerifier.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task NonNullableReferenceTypeParameter_WithNullableAnnotationsEnabled_ProducesDiagnostic()
    {
        string test = """
            #nullable enable
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
    public async Task NumericParameter_CodeFixAddsRequiresRange()
    {
        string test = """
            class Test
            {
                void M(int {|VSV0002:count|})
                {
                    System.Console.WriteLine(count);
                }
            }
            """;

        string fixedCode = """
            using Microsoft;

            class Test
            {
                void M(int count)
                {
                    Requires.Range(count >= 0, nameof(count));
                    System.Console.WriteLine(count);
                }
            }
            """;

        await UseRequiresGuardsVerifier.VerifyCodeFixAsync(test, fixedCode);
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
    public async Task NumericParameter_WithUnrelatedRequiresRangeGuard_ProducesDiagnostic()
    {
        string test = """
            using Microsoft;

            class Test
            {
                void M(int {|VSV0002:count|}, int max)
                {
                    Requires.Range(count <= max, nameof(count));
                    Requires.Range(max >= 0, nameof(max));
                }
            }
            """;

        await UseRequiresGuardsVerifier.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task NumericParameter_WithEquivalentRequiresRangeGuard_ProducesNoDiagnostic()
    {
        string test = """
            using Microsoft;

            class Test
            {
                void M(int count)
                {
                    Requires.Range(0 <= count, nameof(count));
                }
            }
            """;

        await UseRequiresGuardsVerifier.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task ParameterCodeFix_AppendsAfterExistingRequiresGuards()
    {
        string test = """
            using Microsoft;

            class Test
            {
                void M(string value, string {|VSV0001:other|})
                {
                    Requires.NotNull(value);
                    System.Console.WriteLine(value + other);
                }
            }
            """;

        string fixedCode = """
            using Microsoft;

            class Test
            {
                void M(string value, string other)
                {
                    Requires.NotNull(value);
                    Requires.NotNull(other);
                    System.Console.WriteLine(value + other);
                }
            }
            """;

        await UseRequiresGuardsVerifier.VerifyCodeFixAsync(test, fixedCode);
    }

    [Fact]
    public async Task ParameterCodeFix_WithLeadingComment_DoesNotDuplicateTrivia()
    {
        string test = """
            class Test
            {
                void M(string {|VSV0001:value|})
                {
                    // Existing comment
                    System.Console.WriteLine(value);
                }
            }
            """;

        string fixedCode = """
            using Microsoft;

            class Test
            {
                void M(string value)
                {
                    Requires.NotNull(value);
                    // Existing comment
                    System.Console.WriteLine(value);
                }
            }
            """;

        await UseRequiresGuardsVerifier.VerifyCodeFixAsync(test, fixedCode);
    }

    [Fact]
    public async Task ParameterCodeFix_WithMicrosoftAlias_AddsMicrosoftUsing()
    {
        string test = """
            using MS = Microsoft;

            class Test
            {
                void M(string {|VSV0001:value|})
                {
                }
            }
            """;

        string fixedCode = """
            using MS = Microsoft;
            using Microsoft;

            class Test
            {
                void M(string value)
                {
                    Requires.NotNull(value);
                }
            }
            """;

        await UseRequiresGuardsVerifier.VerifyCodeFixAsync(test, fixedCode);
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
    public async Task LocalFunctionParameter_ProducesDiagnostic()
    {
        string test = """
            class Test
            {
                void Outer()
                {
                    void Local(string {|VSV0001:value|})
                    {
                    }
                }
            }
            """;

        await UseRequiresGuardsVerifier.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task ManualNullCheck_CodeFixReplacesIfStatement()
    {
        string test = """
            class Test
            {
                void M(string value)
                {
                    {|VSV0003:if (value == null)
                    {
                        throw new System.ArgumentNullException(nameof(value));
                    }|}
                    System.Console.WriteLine(value);
                }
            }
            """;

        string fixedCode = """
            using Microsoft;

            class Test
            {
                void M(string value)
                {
                    Requires.NotNull(value);
                    System.Console.WriteLine(value);
                }
            }
            """;

        await UseRequiresGuardsVerifier.VerifyCodeFixAsync(test, fixedCode);
    }

    [Fact]
    public async Task ManualNullCheck_WithEscapedIdentifier_PreservesIdentifierText()
    {
        string test = """
            class Test
            {
                void M(string @class)
                {
                    {|VSV0003:if (@class == null)
                    {
                        throw new System.ArgumentNullException(nameof(@class));
                    }|}
                    System.Console.WriteLine(@class);
                }
            }
            """;

        string fixedCode = """
            using Microsoft;

            class Test
            {
                void M(string @class)
                {
                    Requires.NotNull(@class);
                    System.Console.WriteLine(@class);
                }
            }
            """;

        await UseRequiresGuardsVerifier.VerifyCodeFixAsync(test, fixedCode);
    }

    [Fact]
    public async Task ManualNullCheck_WithNullableAnnotationsDisabled_ProducesDiagnostic()
    {
        string test = """
            #nullable disable
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
