Microsoft.VisualStudio.Validation
=================================

[![NuGet package](https://img.shields.io/nuget/v/Microsoft.VisualStudio.Validation.svg)](https://nuget.org/packages/Microsoft.VisualStudio.Validation)
[![Build status](https://ci.appveyor.com/api/projects/status/my8aqxc30x0n2xtl/branch/master?svg=true)](https://ci.appveyor.com/project/AArnott/vs-validation/branch/master)
[![codecov](https://codecov.io/gh/Microsoft/vs-validation/branch/master/graph/badge.svg)](https://codecov.io/gh/Microsoft/vs-validation)

This project is available as the [Microsoft.VisualStudio.Validation][1] NuGet package.

Basic input validation via the `Requires` class throws an ArgumentException.

```csharp
Requires.NotNull(arg1, nameof(arg1));
Requires.NotNullOrEmpty(arg2, nameof(arg2));
```

State validation via the `Verify` class throws an InvalidOperationException.

```csharp
Verify.Operation(condition, "some error occurred.");
```

Internal integrity checks via the `Assumes` class throws an
InternalErrorException.

```csharp
Assumes.True(condition, "some error");
```

Warning signs that should not throw exceptions via the `Report` class.

```csharp
Report.IfNot(condition, "some error");
```

Code Snippets
-------------

Make writing input validation especially convenient with [code snippets][2].
Run the tools\install_snippets.cmd script within this package to copy the code snippets
into your `Documents\Visual Studio 201x\Code Snippets\Visual C#\My Code Snippets`
folder(s) and just type the first few letters of the code snippet name to get
auto-completion assisted input validation.

Note that if you have Resharper installed, code snippets don't appear in
auto-completion lists so you may have to press `Ctrl+J` after the first few letters
of the code snippet name for it to become available.

Example:

```csharp
private void SomeMethod(string input) {
    rnne<TAB>
}
```

Expands to

```csharp
private void SomeMethod(string input) {
    Requires.NotNullOrEmpty(paramName, nameof(paramName));
}
```

And the first `paramName` is selected. Simply type the actual parameter name
(Intellisense will auto-complete for you) and then the quoted paramName name
will automatically be changed to match.

The two snippets are `rnn` and `rnne`
which expand to check for null inputs or null-or-empty inputs, respectively.

[1]: http://nuget.org/packages/Microsoft.VisualStudio.Validation "Microsoft.VisualStudio.Validation NuGet package"
[2]: src\Microsoft.VisualStudio.Validation.NuGet\tools "Code Snippets"
