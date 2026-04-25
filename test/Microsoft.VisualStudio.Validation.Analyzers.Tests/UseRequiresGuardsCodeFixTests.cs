// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.VisualStudio.Validation.Analyzers.Tests.Helpers;
using Xunit;

namespace Microsoft.VisualStudio.Validation.Analyzers.Tests;

public class UseRequiresGuardsCodeFixTests
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
}
