﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ExpressionIsAlwaysEqualToTrueOrFalseRefactoring
    {
        // i >= 0
        public static void GreaterThanOrEqualExpression(SyntaxNodeAnalysisContext context)
        {
            var greaterThanOrEqualExpression = (BinaryExpressionSyntax)context.Node;

            BinaryExpressionInfo info = SyntaxInfo.BinaryExpressionInfo(greaterThanOrEqualExpression);

            if (!info.Success)
                return;

            if (!IsFixable(greaterThanOrEqualExpression, info.Left, info.Right, context.SemanticModel, context.CancellationToken))
                return;

            ReportDiagnostic(context, "false");
        }

        // 0 > i
        public static void GreaterThanExpression(SyntaxNodeAnalysisContext context)
        {
            var greaterThanExpression = (BinaryExpressionSyntax)context.Node;

            BinaryExpressionInfo info = SyntaxInfo.BinaryExpressionInfo(greaterThanExpression);

            if (!info.Success)
                return;

            if (!IsFixable(greaterThanExpression, info.Right, info.Left, context.SemanticModel, context.CancellationToken))
                return;

            ReportDiagnostic(context, "true");
        }

        // 0 <= i
        public static void LessThanOrEqualExpression(SyntaxNodeAnalysisContext context)
        {
            var lessThanOrEqualExpression = (BinaryExpressionSyntax)context.Node;

            BinaryExpressionInfo info = SyntaxInfo.BinaryExpressionInfo(lessThanOrEqualExpression);

            if (!info.Success)
                return;

            if (!IsFixable(lessThanOrEqualExpression, info.Right, info.Left, context.SemanticModel, context.CancellationToken))
                return;

            ReportDiagnostic(context, "true");
        }

        // i < 0
        public static void LessThanExpression(SyntaxNodeAnalysisContext context)
        {
            var lessThanExpression = (BinaryExpressionSyntax)context.Node;

            BinaryExpressionInfo info = SyntaxInfo.BinaryExpressionInfo(lessThanExpression);

            if (!info.Success)
                return;

            if (!IsFixable(lessThanExpression, info.Left, info.Right, context.SemanticModel, context.CancellationToken))
                return;

            ReportDiagnostic(context, "false");
        }

        private static bool IsFixable(
            BinaryExpressionSyntax binaryExpression,
            ExpressionSyntax left,
            ExpressionSyntax right,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (right.Kind() != SyntaxKind.NumericLiteralExpression)
                return false;

            if (right.Span.Length != 1)
                return false;

            var numericLiteralExpression = (LiteralExpressionSyntax)right;

            if (numericLiteralExpression.Token.ValueText != "0")
                return false;

            if (binaryExpression.IsKind(SyntaxKind.LessThanOrEqualExpression, SyntaxKind.GreaterThanOrEqualExpression)
                && IsReversedForStatement(binaryExpression, left))
            {
                return false;
            }

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(left, cancellationToken);

            if (typeSymbol == null)
                return false;

            SpecialType specialType = typeSymbol.SpecialType;

            if (specialType.Is(
                SpecialType.System_Byte,
                SpecialType.System_UInt16,
                SpecialType.System_UInt32,
                SpecialType.System_UInt64))
            {
                return true;
            }
            else if (specialType == SpecialType.System_Int32)
            {
                SyntaxKind kind = left.Kind();

                if (kind == SyntaxKind.SimpleMemberAccessExpression)
                {
                    var memberAccessExpression = (MemberAccessExpressionSyntax)left;

                    var name = memberAccessExpression.Name as IdentifierNameSyntax;

                    switch (name?.Identifier.ValueText)
                    {
                        case "Count":
                        case "Length":
                            {
                                var symbol = semanticModel.GetSymbol(left, cancellationToken) as IPropertySymbol;

                                INamedTypeSymbol containingType = symbol.ContainingType;

                                if (containingType != null)
                                {
                                    if (containingType.IsSpecialType(
                                        SpecialType.System_String,
                                        SpecialType.System_Array,
                                        SpecialType.System_Collections_Generic_ICollection_T))
                                    {
                                        return true;
                                    }

                                    if (containingType?.Implements(SpecialType.System_Collections_Generic_ICollection_T, allInterfaces: true) == true)
                                        return true;
                                }

                                break;
                            }
                    }
                }
            }

            return false;
        }

        private static bool IsReversedForStatement(BinaryExpressionSyntax binaryExpression, ExpressionSyntax left)
        {
            if (!(left is IdentifierNameSyntax identifierName))
                return false;

            if (!(binaryExpression.WalkUpParentheses().Parent is ForStatementSyntax forStatement))
                return false;

            VariableDeclarationSyntax declaration = forStatement.Declaration;

            if (declaration == null)
                return false;

            string name = identifierName.Identifier.ValueText;

            foreach (VariableDeclaratorSyntax declarator in declaration.Variables)
            {
                if (string.Equals(name, declarator.Identifier.ValueText, StringComparison.Ordinal))
                    return true;
            }

            return false;
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, string booleanName)
        {
            context.ReportDiagnostic(
                DiagnosticDescriptors.ExpressionIsAlwaysEqualToTrueOrFalse,
                context.Node,
                booleanName);
        }
    }
}
