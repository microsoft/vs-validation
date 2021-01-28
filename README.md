Microsoft.VisualStudio.Validation
=================================

[![NuGet package](https://img.shields.io/nuget/v/Microsoft.VisualStudio.Validation.svg)](https://nuget.org/packages/Microsoft.VisualStudio.Validation)
[![Build Status](https://dev.azure.com/azure-public/vside/_apis/build/status/vs-validation?branchName=main)](https://dev.azure.com/azure-public/vside/_build/latest?definitionId=11&branchName=main)
[![codecov](https://codecov.io/gh/Microsoft/vs-validation/branch/main/graph/badge.svg)](https://codecov.io/gh/Microsoft/vs-validation)

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

[1]: http://nuget.org/packages/Microsoft.VisualStudio.Validation "Microsoft.VisualStudio.Validation NuGet package"
