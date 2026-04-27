// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Microsoft.VisualStudio.Validation.Analyzers;

/// <summary>
/// Analyzes C# code for opportunities to use Microsoft.Requires guard methods.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class CSharpUseRequiresGuardsAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => DiagnosticDescriptors.All;

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterCompilationStartAction(startContext =>
        {
            if (startContext.Compilation.GetTypeByMetadataName(KnownTypeNames.Requires) is null)
            {
                return;
            }

            var guardedParametersByBody = new ConcurrentDictionary<BlockSyntax, GuardedParameters>();
            startContext.RegisterSyntaxNodeAction<SyntaxKind>(context => AnalyzeParameter(context, guardedParametersByBody), SyntaxKind.Parameter);
            startContext.RegisterSyntaxNodeAction<SyntaxKind>(AnalyzeIfStatement, SyntaxKind.IfStatement);
            startContext.RegisterSyntaxNodeAction<SyntaxKind>(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        });
    }

    private static void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
    {
        var ifStatement = (IfStatementSyntax)context.Node;
        if (ifStatement.Else is not null)
        {
            return;
        }

        if (!TryGetNullCheckedIdentifier(ifStatement.Condition, out IdentifierNameSyntax identifier)
            || context.SemanticModel.GetSymbolInfo(identifier, context.CancellationToken).Symbol is not IParameterSymbol parameterSymbol)
        {
            return;
        }

        ThrowStatementSyntax? throwStatement = ExtractSingleThrowStatement(ifStatement.Statement);
        if (throwStatement?.Expression is null
            || context.SemanticModel.GetOperation(throwStatement.Expression, context.CancellationToken) is not IObjectCreationOperation objectCreation)
        {
            return;
        }

        if (!IsArgumentNullException(objectCreation))
        {
            return;
        }

        if (!TryGetArgumentParameterName(objectCreation, context.CancellationToken, out string? parameterName)
            || !string.Equals(parameterName, parameterSymbol.Name, StringComparison.Ordinal))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.UseRequiresNotNull, ifStatement.GetLocation(), parameterSymbol.Name));
    }

    private static bool TryGetContainingBody(ParameterSyntax parameterSyntax, out BlockSyntax body)
    {
        switch (parameterSyntax.Parent?.Parent)
        {
            case BaseMethodDeclarationSyntax { Body: { } methodBody }:
                body = methodBody;
                return true;
            case LocalFunctionStatementSyntax { Body: { } localFunctionBody }:
                body = localFunctionBody;
                return true;
            default:
                body = null!;
                return false;
        }
    }

    private static bool TryGetContainingParameters(BlockSyntax body, out SeparatedSyntaxList<ParameterSyntax> parameters)
    {
        switch (body.Parent)
        {
            case BaseMethodDeclarationSyntax { ParameterList: { } parameterList }:
                parameters = parameterList.Parameters;
                return true;
            case LocalFunctionStatementSyntax { ParameterList: { } parameterList }:
                parameters = parameterList.Parameters;
                return true;
            default:
                parameters = default;
                return false;
        }
    }

    private static bool TryGetRequiresNotNullGuardedParameter(InvocationExpressionSyntax invocation, SemanticModel semanticModel, IReadOnlyDictionary<string, IParameterSymbol> parametersByName, CancellationToken cancellationToken, out IParameterSymbol parameterSymbol)
    {
        parameterSymbol = null!;

        if (!IsRequiresMethod(invocation, semanticModel, "NotNull", cancellationToken) || invocation.ArgumentList.Arguments.Count == 0)
        {
            return false;
        }

        if (semanticModel.GetSymbolInfo(invocation.ArgumentList.Arguments[0].Expression, cancellationToken).Symbol is not IParameterSymbol argumentSymbol)
        {
            return false;
        }

        if (!parametersByName.TryGetValue(argumentSymbol.Name, out parameterSymbol)
            || !SymbolEqualityComparer.Default.Equals(argumentSymbol, parameterSymbol))
        {
            parameterSymbol = null!;
            return false;
        }

        return true;
    }

    private static bool TryGetArgumentNullCheckGuardedParameter(IfStatementSyntax ifStatement, SemanticModel semanticModel, IReadOnlyDictionary<string, IParameterSymbol> parametersByName, CancellationToken cancellationToken, out IParameterSymbol parameterSymbol)
    {
        parameterSymbol = null!;

        if (ifStatement.Else is not null
            || !TryGetNullCheckedParameterName(ifStatement.Condition, out string? checkedParameterName))
        {
            return false;
        }

        ThrowStatementSyntax? throwStatement = ExtractSingleThrowStatement(ifStatement.Statement);
        if (throwStatement?.Expression is null
            || semanticModel.GetOperation(throwStatement.Expression, cancellationToken) is not IObjectCreationOperation objectCreation)
        {
            return false;
        }

        if (!IsArgumentNullException(objectCreation)
            || !TryGetArgumentParameterName(objectCreation, cancellationToken, out string thrownParameterName)
            || !string.Equals(checkedParameterName, thrownParameterName, StringComparison.Ordinal))
        {
            return false;
        }

        return checkedParameterName is not null && parametersByName.TryGetValue(checkedParameterName, out parameterSymbol);
    }

    private static bool TryGetRequiresRangeGuardedParameter(InvocationExpressionSyntax invocation, SemanticModel semanticModel, IReadOnlyDictionary<string, IParameterSymbol> parametersByName, CancellationToken cancellationToken, out IParameterSymbol parameterSymbol)
    {
        parameterSymbol = null!;

        if (!IsRequiresMethod(invocation, semanticModel, "Range", cancellationToken) || invocation.ArgumentList.Arguments.Count < 2)
        {
            return false;
        }

        if (!TryGetParameterNameExpression(invocation.ArgumentList.Arguments[1].Expression, semanticModel, cancellationToken, out string parameterName)
            || !parametersByName.TryGetValue(parameterName, out parameterSymbol)
            || !IsNonNegativeRangeCheck(invocation.ArgumentList.Arguments[0].Expression, semanticModel, parameterSymbol, cancellationToken))
        {
            parameterSymbol = null!;
            return false;
        }

        return true;
    }

    private static bool IsNonNegativeRangeCheck(ExpressionSyntax expression, SemanticModel semanticModel, IParameterSymbol parameterSymbol, CancellationToken cancellationToken)
    {
        expression = Unwrap(expression);
        return expression switch
        {
            BinaryExpressionSyntax { RawKind: (int)SyntaxKind.GreaterThanOrEqualExpression, Left: var left, Right: var right }
                => IsDirectParameterReference(left, semanticModel, parameterSymbol, cancellationToken) && IsZeroConstant(right, semanticModel, cancellationToken),
            BinaryExpressionSyntax { RawKind: (int)SyntaxKind.LessThanOrEqualExpression, Left: var left, Right: var right }
                => IsZeroConstant(left, semanticModel, cancellationToken) && IsDirectParameterReference(right, semanticModel, parameterSymbol, cancellationToken),
            _ => false,
        };
    }

    private static bool IsDirectParameterReference(ExpressionSyntax expression, SemanticModel semanticModel, IParameterSymbol parameterSymbol, CancellationToken cancellationToken)
        => Unwrap(expression) is IdentifierNameSyntax identifier
            && SymbolEqualityComparer.Default.Equals(semanticModel.GetSymbolInfo(identifier, cancellationToken).Symbol, parameterSymbol);

    private static bool IsZeroConstant(ExpressionSyntax expression, SemanticModel semanticModel, CancellationToken cancellationToken)
    {
        expression = Unwrap(expression);
        Optional<object?> constantValue = semanticModel.GetConstantValue(expression, cancellationToken);
        if (!constantValue.HasValue || constantValue.Value is null)
        {
            return false;
        }

        return constantValue.Value switch
        {
            sbyte value => value == 0,
            byte value => value == 0,
            short value => value == 0,
            ushort value => value == 0,
            int value => value == 0,
            uint value => value == 0,
            long value => value == 0,
            ulong value => value == 0,
            float value => value == 0,
            double value => value == 0,
            decimal value => value == 0,
            _ => false,
        };
    }

    private static ExpressionSyntax Unwrap(ExpressionSyntax expression)
        => expression is ParenthesizedExpressionSyntax { Expression: { } innerExpression }
            ? Unwrap(innerExpression)
            : expression;

    private static bool IsRequiresMethod(InvocationExpressionSyntax invocation, SemanticModel semanticModel, string methodName, CancellationToken cancellationToken)
    {
        if (semanticModel.GetSymbolInfo(invocation, cancellationToken).Symbol is not IMethodSymbol method)
        {
            return false;
        }

        return method.Name == methodName
            && method.ContainingType?.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat) == KnownTypeNames.Requires;
    }

    private static bool TryGetRedundantNotNullParameterNameArgument(InvocationExpressionSyntax invocation, SemanticModel semanticModel, CancellationToken cancellationToken, out ArgumentSyntax redundantArgument)
    {
        redundantArgument = null!;

        if (!IsRequiresMethod(invocation, semanticModel, "NotNull", cancellationToken)
            || semanticModel.GetOperation(invocation, cancellationToken) is not IInvocationOperation operation)
        {
            return false;
        }

        IArgumentOperation? valueArgument = operation.Arguments.FirstOrDefault(arg => string.Equals(arg.Parameter?.Name, "value", StringComparison.Ordinal));
        IArgumentOperation? parameterNameArgument = operation.Arguments.FirstOrDefault(arg => string.Equals(arg.Parameter?.Name, "parameterName", StringComparison.Ordinal) && !arg.IsImplicit);
        if (valueArgument?.Syntax is not ArgumentSyntax { Expression: ExpressionSyntax valueExpression }
            || parameterNameArgument?.Syntax is not ArgumentSyntax explicitParameterNameArgument)
        {
            return false;
        }

        Optional<object?> constantValue = parameterNameArgument.Value.ConstantValue;
        if (!constantValue.HasValue || constantValue.Value is not string parameterName)
        {
            return false;
        }

        if (!string.Equals(parameterName, valueExpression.ToString(), StringComparison.Ordinal))
        {
            return false;
        }

        redundantArgument = explicitParameterNameArgument;
        return true;
    }

    private static bool TryGetParameterNameExpression(ExpressionSyntax expression, SemanticModel semanticModel, CancellationToken cancellationToken, out string parameterName)
    {
        Optional<object?> constantValue = semanticModel.GetConstantValue(expression, cancellationToken);
        if (constantValue.HasValue && constantValue.Value is string constantParameterName)
        {
            parameterName = constantParameterName;
            return true;
        }

        if (expression is InvocationExpressionSyntax nameofInvocation
            && nameofInvocation.Expression is IdentifierNameSyntax { Identifier.ValueText: "nameof" }
            && nameofInvocation.ArgumentList.Arguments.Count == 1
            && nameofInvocation.ArgumentList.Arguments[0].Expression is IdentifierNameSyntax identifier)
        {
            parameterName = identifier.Identifier.ValueText;
            return true;
        }

        parameterName = null!;
        return false;
    }

    private static bool IsSupportedNumericType(ITypeSymbol type)
        => type.SpecialType is
            SpecialType.System_SByte or
            SpecialType.System_Int16 or
            SpecialType.System_Int32 or
            SpecialType.System_Int64 or
            SpecialType.System_Single or
            SpecialType.System_Double or
            SpecialType.System_Decimal;

    private static bool TryGetNullCheckedIdentifier(ExpressionSyntax condition, out IdentifierNameSyntax identifier)
    {
        switch (condition)
        {
            case BinaryExpressionSyntax { Left: IdentifierNameSyntax left, Right: LiteralExpressionSyntax right } when right.IsKind(SyntaxKind.NullLiteralExpression):
                identifier = left;
                return true;
            case BinaryExpressionSyntax { Right: IdentifierNameSyntax right, Left: LiteralExpressionSyntax left } when left.IsKind(SyntaxKind.NullLiteralExpression):
                identifier = right;
                return true;
            case IsPatternExpressionSyntax { Expression: IdentifierNameSyntax candidate, Pattern: ConstantPatternSyntax { Expression: LiteralExpressionSyntax literal } } when literal.IsKind(SyntaxKind.NullLiteralExpression):
                identifier = candidate;
                return true;
            default:
                identifier = null!;
                return false;
        }
    }

    private static bool TryGetNullCheckedParameterName(ExpressionSyntax condition, out string? parameterName)
    {
        if (TryGetNullCheckedIdentifier(condition, out IdentifierNameSyntax identifier))
        {
            parameterName = identifier.Identifier.ValueText;
            return true;
        }

        parameterName = null;
        return false;
    }

    private static bool IsArgumentNullException(IObjectCreationOperation objectCreation)
        => objectCreation.Type?.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat) == typeof(ArgumentNullException).FullName;

    private static bool TryGetArgumentParameterName(IObjectCreationOperation objectCreation, CancellationToken cancellationToken, out string parameterName)
    {
        if (objectCreation.Arguments.Length == 0)
        {
            parameterName = null!;
            return false;
        }

        IArgumentOperation argument = objectCreation.Arguments[0];
        Optional<object?> constantValue = argument.Value.ConstantValue;
        if (constantValue.HasValue && constantValue.Value is string constantParameterName)
        {
            parameterName = constantParameterName;
            return true;
        }

        SyntaxNode syntax = argument.Syntax;
        if (syntax is ArgumentSyntax { Expression: InvocationExpressionSyntax nameofInvocation }
            && nameofInvocation.Expression is IdentifierNameSyntax { Identifier.ValueText: "nameof" }
            && nameofInvocation.ArgumentList.Arguments.Count == 1
            && nameofInvocation.ArgumentList.Arguments[0].Expression is IdentifierNameSyntax identifier)
        {
            parameterName = identifier.Identifier.ValueText;
            return true;
        }

        parameterName = null!;
        return false;
    }

    private static ThrowStatementSyntax? ExtractSingleThrowStatement(StatementSyntax statement)
        => statement switch
        {
            BlockSyntax { Statements.Count: 1 } block => block.Statements[0] as ThrowStatementSyntax,
            ThrowStatementSyntax throwStatement => throwStatement,
            _ => null,
        };

    private static DiagnosticDescriptor? GetParameterDescriptor(IParameterSymbol parameterSymbol)
    {
        if (parameterSymbol.RefKind != RefKind.None || parameterSymbol.IsParams)
        {
            return null;
        }

        if (parameterSymbol.Type.IsReferenceType)
        {
            return parameterSymbol.NullableAnnotation == NullableAnnotation.Annotated
                ? null
                : DiagnosticDescriptors.AddRequiresNotNull;
        }

        if (parameterSymbol.Type is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T })
        {
            return null;
        }

        if (IsSupportedNumericType(parameterSymbol.Type))
        {
            return DiagnosticDescriptors.AddRequiresRange;
        }

        return null;
    }

    private static void AnalyzeParameter(SyntaxNodeAnalysisContext context, ConcurrentDictionary<BlockSyntax, GuardedParameters> guardedParametersByBody)
    {
        var parameterSyntax = (ParameterSyntax)context.Node;
        if (parameterSyntax.Identifier.IsMissing || !TryGetContainingBody(parameterSyntax, out BlockSyntax body))
        {
            return;
        }

        if (context.SemanticModel.GetDeclaredSymbol(parameterSyntax, context.CancellationToken) is not IParameterSymbol parameterSymbol)
        {
            return;
        }

        DiagnosticDescriptor? descriptor = GetParameterDescriptor(parameterSymbol);
        if (descriptor is null)
        {
            return;
        }

        GuardedParameters guardedParameters = guardedParametersByBody.GetOrAdd(body, _ => ScanGuardedParameters(body, context.SemanticModel, context.CancellationToken));
        if (descriptor == DiagnosticDescriptors.AddRequiresNotNull && guardedParameters.NotNullParameters.Contains(parameterSymbol))
        {
            return;
        }

        if (descriptor == DiagnosticDescriptors.AddRequiresRange && guardedParameters.RangeParameters.Contains(parameterSymbol))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(descriptor, parameterSyntax.Identifier.GetLocation(), parameterSymbol.Name));
    }

    private static GuardedParameters ScanGuardedParameters(BlockSyntax body, SemanticModel semanticModel, CancellationToken cancellationToken)
    {
        var guardedParameters = new GuardedParameters();
        if (!TryGetContainingParameters(body, out SeparatedSyntaxList<ParameterSyntax> parameters))
        {
            return guardedParameters;
        }

        var parametersByName = new Dictionary<string, IParameterSymbol>(StringComparer.Ordinal);
        foreach (ParameterSyntax parameterSyntax in parameters)
        {
            if (parameterSyntax.Identifier.IsMissing
                || semanticModel.GetDeclaredSymbol(parameterSyntax, cancellationToken) is not IParameterSymbol parameterSymbol)
            {
                continue;
            }

            DiagnosticDescriptor? descriptor = GetParameterDescriptor(parameterSymbol);
            if (descriptor is null)
            {
                continue;
            }

            parametersByName[parameterSymbol.Name] = parameterSymbol;
        }

        if (parametersByName.Count == 0)
        {
            return guardedParameters;
        }

        foreach (StatementSyntax statement in body.Statements)
        {
            if (statement is IfStatementSyntax ifStatement
                && TryGetArgumentNullCheckGuardedParameter(ifStatement, semanticModel, parametersByName, cancellationToken, out IParameterSymbol guardedNotNullParameter))
            {
                guardedParameters.NotNullParameters.Add(guardedNotNullParameter);
            }

            if (statement is not ExpressionStatementSyntax { Expression: InvocationExpressionSyntax invocation })
            {
                continue;
            }

            if (TryGetRequiresNotNullGuardedParameter(invocation, semanticModel, parametersByName, cancellationToken, out guardedNotNullParameter))
            {
                guardedParameters.NotNullParameters.Add(guardedNotNullParameter);
            }

            if (TryGetRequiresRangeGuardedParameter(invocation, semanticModel, parametersByName, cancellationToken, out IParameterSymbol guardedRangeParameter))
            {
                guardedParameters.RangeParameters.Add(guardedRangeParameter);
            }
        }

        return guardedParameters;
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;
        if (!TryGetRedundantNotNullParameterNameArgument(invocation, context.SemanticModel, context.CancellationToken, out ArgumentSyntax redundantArgument))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.RemoveRedundantNotNullParameterName, redundantArgument.GetLocation()));
    }

    private sealed class GuardedParameters
    {
        internal HashSet<IParameterSymbol> NotNullParameters { get; } = new(SymbolEqualityComparer.Default);

        internal HashSet<IParameterSymbol> RangeParameters { get; } = new(SymbolEqualityComparer.Default);
    }
}
