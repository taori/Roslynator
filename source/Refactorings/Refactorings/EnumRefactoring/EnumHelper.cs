﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings.EnumWithFlagsAttribute
{
    internal static class EnumHelper
    {
        public static EnumMemberDeclarationSyntax CreateEnumMember(INamedTypeSymbol enumSymbol, string name, object value)
        {
            EqualsValueClauseSyntax equalsValue = null;

            if (value != null)
                equalsValue = EqualsValueClause(ConstantExpression(value));

            name = Identifier.EnsureUniqueEnumMemberName(enumSymbol, name);

            SyntaxToken identifier = Identifier(name).WithRenameAnnotation();

            return EnumMemberDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                identifier,
                equalsValue);
        }

        public static List<object> GetExplicitValues(
            EnumDeclarationSyntax enumDeclaration,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            var values = new List<object>();

            foreach (EnumMemberDeclarationSyntax member in enumDeclaration.Members)
            {
                EqualsValueClauseSyntax equalsValue = member.EqualsValue;

                if (equalsValue != null)
                {
                    ExpressionSyntax value = equalsValue.Value;

                    if (value != null)
                    {
                        var fieldSymbol = semanticModel.GetDeclaredSymbol(member, cancellationToken) as IFieldSymbol;

                        if (fieldSymbol?.HasConstantValue == true)
                            values.Add(fieldSymbol.ConstantValue);
                    }
                }
            }

            return values;
        }

        public static IEnumerable<object> GetValues(
            ITypeSymbol enumSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            foreach (ISymbol member in enumSymbol.GetMembers())
            {
                if (member.IsField())
                {
                    var fieldSymbol = (IFieldSymbol)member;

                    if (fieldSymbol.HasConstantValue)
                        yield return fieldSymbol.ConstantValue;
                }
            }
        }

        public static bool IsValueDefined(
            object value,
            INamedTypeSymbol enumSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            foreach (object value2 in GetValues(enumSymbol, semanticModel, cancellationToken))
            {
                if (value2.Equals(value))
                    return true;
            }

            return false;
        }
    }
}