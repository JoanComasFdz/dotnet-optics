using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JoanComasFdz.Optics.Lenses.v1.SourceGenerated;

public partial class LensSourceGenerator
{
    private class LensesSyntaxReceiver : ISyntaxReceiver
    {
        public List<TypeDeclarationSyntax> CandidateTypes { get; } = [];

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            // Look for class or record declarations with the [Lenses] attribute
            if (syntaxNode is TypeDeclarationSyntax typeDeclarationSyntax)
            {
                if (typeDeclarationSyntax.AttributeLists.Any(al => al.Attributes.Any(a => a.Name.ToString() == "Lenses")))
                {
                    CandidateTypes.Add(typeDeclarationSyntax);
                }
            }
        }
    }
}
