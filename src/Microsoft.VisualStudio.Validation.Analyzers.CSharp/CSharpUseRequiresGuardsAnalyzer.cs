// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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

            startContext.RegisterSyntaxNodeAction<SyntaxKind>(AnalyzeParameter, SyntaxKind.Parameter);
            startContext.RegisterSyntaxNodeAction<SyntaxKind>(AnalyzeIfStatement, SyntaxKind.IfStatement);
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

        if (!HasMatchingParameterNameArgument(objectCreation, parameterSymbol, context.CancellationToken))
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

    private static bool HasRequiresNotNullGuard(BlockSyntax body, SemanticModel semanticModel, IParameterSymbol parameterSymbol, CancellationToken cancellationToken)
    {
        foreach (StatementSyntax statement in body.Statements)
        {
            if (statement is IfStatementSyntax ifStatement
                && IsArgumentNullCheckGuard(ifStatement, semanticModel, parameterSymbol, cancellationToken))
            {
                return true;
            }

            if (statement is not ExpressionStatementSyntax { Expression: InvocationExpressionSyntax invocation })
            {
                continue;
            }

            if (!IsRequiresMethod(invocation, semanticModel, "NotNull", cancellationToken) || invocation.ArgumentList.Arguments.Count == 0)
            {
                continue;
            }

            if (semanticModel.GetSymbolInfo(invocation.ArgumentList.Arguments[0].Expression, cancellationToken).Symbol is IParameterSymbol argumentSymbol
                && SymbolEqualityComparer.Default.Equals(argumentSymbol, parameterSymbol))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsArgumentNullCheckGuard(IfStatementSyntax ifStatement, SemanticModel semanticModel, IParameterSymbol parameterSymbol, CancellationToken cancellationToken)
    {
        if (ifStatement.Else is not null || !IsNullCheckForParameter(ifStatement.Condition, parameterSymbol.Name))
        {
            return false;
        }

        ThrowStatementSyntax? throwStatement = ExtractSingleThrowStatement(ifStatement.Statement);
        if (throwStatement?.Expression is null
            || semanticModel.GetOperation(throwStatement.Expression, cancellationToken) is not IObjectCreationOperation objectCreation)
        {
            return false;
        }

        return IsArgumentNullException(objectCreation)
            && HasMatchingParameterNameArgument(objectCreation, parameterSymbol, cancellationToken);
    }

    private static bool HasRequiresRangeGuard(BlockSyntax body, SemanticModel semanticModel, IParameterSymbol parameterSymbol, CancellationToken cancellationToken)
    {
        foreach (StatementSyntax statement in body.Statements)
        {
            if (statement is not ExpressionStatementSyntax { Expression: InvocationExpressionSyntax invocation })
            {
                continue;
            }

            if (!IsRequiresMethod(invocation, semanticModel, "Range", cancellationToken) || invocation.ArgumentList.Arguments.Count < 2)
            {
                continue;
            }

            if (ExpressionReferencesParameter(invocation.ArgumentList.Arguments[0].Expression, semanticModel, parameterSymbol, cancellationToken)
                && IsParameterNameExpression(invocation.ArgumentList.Arguments[1].Expression, semanticModel, parameterSymbol, cancellationToken))
            {
                return true;
            }
        }

        return false;
    }

    private static bool ExpressionReferencesParameter(ExpressionSyntax expression, SemanticModel semanticModel, IParameterSymbol parameterSymbol, CancellationToken cancellationToken)
        => expression.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>().Any(identifier =>
            SymbolEqualityComparer.Default.Equals(
                semanticModel.GetSymbolInfo(identifier, cancellationToken).Symbol,
                parameterSymbol));

    private static bool IsRequiresMethod(InvocationExpressionSyntax invocation, SemanticModel semanticModel, string methodName, CancellationToken cancellationToken)
    {
        if (semanticModel.GetSymbolInfo(invocation, cancellationToken).Symbol is not IMethodSymbol method)
        {
            return false;
        }

        return method.Name == methodName
            && method.ContainingType?.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat) == KnownTypeNames.Requires;
    }

    private static bool IsParameterNameExpression(ExpressionSyntax expression, SemanticModel semanticModel, IParameterSymbol parameterSymbol, CancellationToken cancellationToken)
    {
        Optional<object?> constantValue = semanticModel.GetConstantValue(expression, cancellationToken);
        if (constantValue.HasValue && string.Equals(constantValue.Value as string, parameterSymbol.Name, StringComparison.Ordinal))
        {
            return true;
        }

        if (expression is InvocationExpressionSyntax nameofInvocation
            && nameofInvocation.Expression is IdentifierNameSyntax { Identifier.ValueText: "nameof" }
            && nameofInvocation.ArgumentList.Arguments.Count == 1
            && nameofInvocation.ArgumentList.Arguments[0].Expression is IdentifierNameSyntax identifier)
        {
            return string.Equals(identifier.Identifier.ValueText, parameterSymbol.Name, StringComparison.Ordinal);
        }

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

    private static bool IsNullCheckForParameter(ExpressionSyntax condition, string parameterName)
        => TryGetNullCheckedParameterName(condition, out string? actualParameterName)
            && actualParameterName == parameterName;

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

    private static bool HasMatchingParameterNameArgument(IObjectCreationOperation objectCreation, IParameterSymbol parameterSymbol, CancellationToken cancellationToken)
    {
        if (objectCreation.Arguments.Length == 0)
        {
            return false;
        }

        IArgumentOperation argument = objectCreation.Arguments[0];
        Optional<object?> constantValue = argument.Value.ConstantValue;
        if (constantValue.HasValue && string.Equals(constantValue.Value as string, parameterSymbol.Name, StringComparison.Ordinal))
        {
            return true;
        }

        SyntaxNode syntax = argument.Syntax;
        if (syntax is ArgumentSyntax { Expression: InvocationExpressionSyntax nameofInvocation }
            && nameofInvocation.Expression is IdentifierNameSyntax { Identifier.ValueText: "nameof" }
            && nameofInvocation.ArgumentList.Arguments.Count == 1
            && nameofInvocation.ArgumentList.Arguments[0].Expression is IdentifierNameSyntax identifier)
        {
            return string.Equals(identifier.Identifier.ValueText, parameterSymbol.Name, StringComparison.Ordinal);
        }

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
            return DiagnosticDescriptors.AddRequiresNotNull;
        }

        ITypeSymbol type = parameterSymbol.Type is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } nullableType
            ? nullableType.TypeArguments[0]
            : parameterSymbol.Type;

        if (IsSupportedNumericType(type))
        {
            return DiagnosticDescriptors.AddRequiresRange;
        }

        return null;
    }

    private static void AnalyzeParameter(SyntaxNodeAnalysisContext context)
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

        if (descriptor == DiagnosticDescriptors.AddRequiresNotNull && HasRequiresNotNullGuard(body, context.SemanticModel, parameterSymbol, context.CancellationToken))
        {
            return;
        }

        if (descriptor == DiagnosticDescriptors.AddRequiresRange && HasRequiresRangeGuard(body, context.SemanticModel, parameterSymbol, context.CancellationToken))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(descriptor, parameterSyntax.Identifier.GetLocation(), parameterSymbol.Name));
    }
}
