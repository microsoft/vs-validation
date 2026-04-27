# Microsoft.VisualStudio.Validation.Analyzers

This package contains Roslyn analyzers and code fixes for code that uses the [Microsoft.VisualStudio.Validation package](https://www.nuget.org/packages/Microsoft.VisualStudio.Validation).

Analyzer ID | Description
--|--
VSV0001 | Add `Requires.NotNull` for a reference-type parameter.
VSV0002 | Add `Requires.Range` for a supported numeric parameter.
VSV0003 | Replace a manual null check with `Requires.NotNull`.
VSV0004 | Remove a redundant parameter-name argument from `Requires.NotNull`.
