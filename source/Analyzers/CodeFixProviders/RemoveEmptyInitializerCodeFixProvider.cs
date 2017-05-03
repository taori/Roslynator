﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemoveEmptyInitializerCodeFixProvider))]
    [Shared]
    public class RemoveEmptyInitializerCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.RemoveEmptyInitializer); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            ObjectCreationExpressionSyntax objectCreationExpression = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<ObjectCreationExpressionSyntax>();

            if (objectCreationExpression == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Remove empty initializer",
                cancellationToken => RemoveEmptyInitializerRefactoring.RefactorAsync(context.Document, objectCreationExpression, cancellationToken),
                DiagnosticIdentifiers.RemoveEmptyInitializer + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}
