// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.VisualStudio.Validation.Analyzers;

/// <summary>
/// Applies fixes for diagnostics that recommend Microsoft.Requires guard methods.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CSharpUseRequiresGuardsCodeFixProvider))]
[Shared]
public class CSharpUseRequiresGuardsCodeFixProvider : CodeFixProvider
{
    /// <inheritdoc />
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
        DiagnosticIds.AddRequiresNotNull,
        DiagnosticIds.AddRequiresRange,
        DiagnosticIds.UseRequiresNotNull);

    /// <inheritdoc />
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <inheritdoc />
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode? root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return;
        }

        Diagnostic diagnostic = context.Diagnostics[0];
        SyntaxNode node = root.FindNode(context.Span, getInnermostNodeForTie: true);

        switch (diagnostic.Id)
        {
            case DiagnosticIds.AddRequiresNotNull:
                if (node.FirstAncestorOrSelf<ParameterSyntax>() is ParameterSyntax notNullParameter)
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            "Add Requires.NotNull",
                            cancellationToken => AddParameterGuardAsync(context.Document, notNullParameter, useRange: false, cancellationToken),
                            nameof(DiagnosticIds.AddRequiresNotNull)),
                        diagnostic);
                }

                break;

            case DiagnosticIds.AddRequiresRange:
                if (node.FirstAncestorOrSelf<ParameterSyntax>() is ParameterSyntax rangeParameter)
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            "Add Requires.Range",
                            cancellationToken => AddParameterGuardAsync(context.Document, rangeParameter, useRange: true, cancellationToken),
                            nameof(DiagnosticIds.AddRequiresRange)),
                        diagnostic);
                }

                break;

            case DiagnosticIds.UseRequiresNotNull:
                if (node.FirstAncestorOrSelf<IfStatementSyntax>() is IfStatementSyntax ifStatement)
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            "Replace with Requires.NotNull",
                            cancellationToken => ReplaceManualNullCheckAsync(context.Document, ifStatement, cancellationToken),
                            nameof(DiagnosticIds.UseRequiresNotNull)),
                        diagnostic);
                }

                break;
        }
    }

    private static async Task<Document> AddParameterGuardAsync(Document document, ParameterSyntax parameter, bool useRange, CancellationToken cancellationToken)
    {
        SyntaxNode? root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null || !TryGetContainingBody(parameter, out BlockSyntax body))
        {
            return document;
        }

        string newLine = GetPreferredNewLine(root);
        string parameterName = parameter.Identifier.Text;
        int insertionIndex = GetInsertionIndex(body);
        StatementSyntax guardStatement = CreateGuardStatement(body, parameterName, useRange, insertionIndex, newLine);
        BlockSyntax newBody = body.WithStatements(body.Statements.Insert(insertionIndex, guardStatement));
        SyntaxNode newRoot = root.ReplaceNode(body, newBody);

        if (newRoot is CompilationUnitSyntax compilationUnit)
        {
            newRoot = AddUsingIfMissing(compilationUnit, newLine);
        }

        return document.WithSyntaxRoot(newRoot);
    }

    private static async Task<Document> ReplaceManualNullCheckAsync(Document document, IfStatementSyntax ifStatement, CancellationToken cancellationToken)
    {
        SyntaxNode? root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return document;
        }

        string? parameterName = ExtractParameterName(ifStatement.Condition);
        if (parameterName is null)
        {
            return document;
        }

        string newLine = GetPreferredNewLine(root);
        StatementSyntax replacement = ParseGuardStatement(parameterName, useRange: false)
            .WithLeadingTrivia(ifStatement.GetLeadingTrivia())
            .WithTrailingTrivia(ifStatement.GetTrailingTrivia());

        SyntaxNode newRoot = root.ReplaceNode(ifStatement, replacement);
        if (newRoot is CompilationUnitSyntax compilationUnit)
        {
            newRoot = AddUsingIfMissing(compilationUnit, newLine);
        }

        return document.WithSyntaxRoot(newRoot);
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

    private static int GetInsertionIndex(BlockSyntax body)
    {
        int index = 0;
        while (index < body.Statements.Count && IsLeadingRequiresStatement(body.Statements[index]))
        {
            index++;
        }

        return index;
    }

    private static bool IsLeadingRequiresStatement(StatementSyntax statement)
        => statement is ExpressionStatementSyntax
        {
            Expression: InvocationExpressionSyntax
            {
                Expression: MemberAccessExpressionSyntax
                {
                    Expression: IdentifierNameSyntax { Identifier.ValueText: "Requires" }
                }
            }
        };

    private static StatementSyntax CreateGuardStatement(BlockSyntax body, string parameterName, bool useRange, int insertionIndex, string newLine)
    {
        return ParseGuardStatement(parameterName, useRange)
            .WithLeadingTrivia(GetStatementIndentation(body, insertionIndex))
            .WithTrailingTrivia(SyntaxFactory.EndOfLine(newLine));
    }

    private static SyntaxTriviaList GetStatementIndentation(BlockSyntax body, int insertionIndex)
    {
        if (insertionIndex < body.Statements.Count)
        {
            SyntaxTriviaList leadingTrivia = body.Statements[insertionIndex].GetLeadingTrivia();
            int lastEndOfLine = -1;
            for (int i = 0; i < leadingTrivia.Count; i++)
            {
                if (leadingTrivia[i].IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    lastEndOfLine = i;
                }
            }

            IEnumerable<SyntaxTrivia> indentationTrivia = leadingTrivia
                .Skip(lastEndOfLine + 1)
                .TakeWhile(t => t.IsKind(SyntaxKind.WhitespaceTrivia));
            string indentation = string.Concat(indentationTrivia.Select(t => t.ToString()));
            if (indentation.Length > 0)
            {
                return SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(indentation));
            }
        }

        string closingIndentation = string.Concat(body.CloseBraceToken.LeadingTrivia.Where(t => t.IsKind(SyntaxKind.WhitespaceTrivia)).Select(t => t.ToString()));
        return SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(closingIndentation + "    "));
    }

    private static StatementSyntax ParseGuardStatement(string parameterName, bool useRange)
    {
        string statementText = useRange
            ? $"Requires.Range({parameterName} >= 0, nameof({parameterName}));"
            : $"Requires.NotNull({parameterName});";
        return SyntaxFactory.ParseStatement(statementText)
            .WithTrailingTrivia(SyntaxFactory.ElasticLineFeed);
    }

    private static CompilationUnitSyntax AddUsingIfMissing(CompilationUnitSyntax root, string newLine)
    {
        if (root.Usings.Any(u => !u.StaticKeyword.IsKind(SyntaxKind.StaticKeyword) && string.Equals(u.Name?.ToString(), "Microsoft", StringComparison.Ordinal)))
        {
            return root;
        }

        UsingDirectiveSyntax usingDirective = SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("Microsoft"))
            .WithTrailingTrivia(SyntaxFactory.EndOfLine(newLine), SyntaxFactory.EndOfLine(newLine));
        return root.AddUsings(usingDirective);
    }

    private static string GetPreferredNewLine(SyntaxNode root)
    {
        string text = root.SyntaxTree.GetText().ToString();
        return text.Contains("\r\n", StringComparison.Ordinal) ? "\r\n" : "\n";
    }

    private static string? ExtractParameterName(ExpressionSyntax condition)
        => condition switch
        {
            BinaryExpressionSyntax { Left: IdentifierNameSyntax left, Right: LiteralExpressionSyntax right } when right.IsKind(SyntaxKind.NullLiteralExpression) => left.Identifier.Text,
            BinaryExpressionSyntax { Right: IdentifierNameSyntax right, Left: LiteralExpressionSyntax left } when left.IsKind(SyntaxKind.NullLiteralExpression) => right.Identifier.Text,
            IsPatternExpressionSyntax { Expression: IdentifierNameSyntax identifier, Pattern: ConstantPatternSyntax { Expression: LiteralExpressionSyntax literal } } when literal.IsKind(SyntaxKind.NullLiteralExpression) => identifier.Identifier.Text,
            _ => null,
        };
}
