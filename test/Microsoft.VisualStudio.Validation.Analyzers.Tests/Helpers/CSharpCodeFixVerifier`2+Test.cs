// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Microsoft.VisualStudio.Validation.Analyzers.Tests.Helpers;

internal static partial class CSharpCodeFixVerifier<TAnalyzer, TCodeFix>
{
    internal class Test : CSharpCodeFixTest<TAnalyzer, TCodeFix, XUnitVerifier>
    {
        internal Test()
        {
            this.ReferenceAssemblies = ReferencesHelper.References;
            this.SolutionTransforms.Add((solution, projectId) =>
            {
                solution = solution.WithOptions(solution.Options.WithChangedOption(FormattingOptions.NewLine, LanguageNames.CSharp, "\n"));
                return solution.AddMetadataReference(projectId, MetadataReference.CreateFromFile(typeof(Requires).Assembly.Location));
            });
        }

        internal DiagnosticDescriptor? ExpectedDescriptor { get; set; }

        protected override DiagnosticDescriptor? GetDefaultDiagnostic(DiagnosticAnalyzer[] analyzers)
            => this.ExpectedDescriptor ?? base.GetDefaultDiagnostic(analyzers);
    }
}
