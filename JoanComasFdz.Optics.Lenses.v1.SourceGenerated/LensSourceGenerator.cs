using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JoanComasFdz.Optics.Lenses.v1.SourceGenerated;

[Generator]
public class LensSourceGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForPostInitialization(ctx => ctx.AddSource("LensesAttribute.g.cs", SourceText.From(LensesAttributeCode.Attribute, Encoding.UTF8)));

        context.RegisterForSyntaxNotifications(() => new LensesSyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not LensesSyntaxReceiver receiver)
            return;

        foreach (var typeDeclaration in receiver.CandidateTypes)
        {
            var semanticModel = context.Compilation.GetSemanticModel(typeDeclaration.SyntaxTree);
            var typeSymbol = semanticModel.GetDeclaredSymbol(typeDeclaration) as INamedTypeSymbol;
            if (typeSymbol == null)
                continue;

            // Generate the lenses for this type
            var sourceText = GenerateLensClass(typeSymbol);
            context.AddSource($"{typeSymbol.Name}Lenses.g.cs", SourceText.From(sourceText, Encoding.UTF8));
        }
    }

    private static string GenerateLensClass(INamedTypeSymbol typeSymbol)
    {
        var namespaceName = typeSymbol.ContainingNamespace.ToDisplayString();
        var typeName = typeSymbol.Name;
        var className = $"{typeName}Lenses";

        var sb = new StringBuilder();

        sb.AppendLine("using JoanComasFdz.Optics.Lenses.v1.Fluent;");
        sb.AppendLine();

        if (namespaceName != "<global namespace>")
        {
            sb.AppendLine($"namespace {namespaceName}");
            sb.AppendLine("{");
        }

        sb.AppendLine($"    public static class {className}");
        sb.AppendLine("    {");

        GenerateLensMethodsForRootType(typeSymbol, sb, typeName);

        sb.AppendLine("    }");

        if (namespaceName != "<global namespace>")
        {
            sb.AppendLine("}");
        }

        return sb.ToString();
    }

    private static void GenerateLensMethodsForRootType(INamedTypeSymbol typeSymbol, StringBuilder sb, string rootTypeName)
    {
        var properties = typeSymbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(p => p.Name != "EqualityContract" && !ShouldSkipLens(p.Type))
            .ToArray();

        foreach (var property in properties)
        {
            var currentPropertyName = property.Name;
            var currentPropertyTypeName = property.Type.ToDisplayString();

            // Generate lens method for the root type
            sb.AppendLine($"        public static LensWrapper<{rootTypeName}, {currentPropertyTypeName}> {currentPropertyName}Lens(this {rootTypeName} instance)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var lens = new Lens<{rootTypeName}, {currentPropertyTypeName}>(");
            sb.AppendLine($"                get => instance.{currentPropertyName},");
            sb.AppendLine($"                (whole, part) => whole with {{ {currentPropertyName} = part }});");
            sb.AppendLine($"            return new LensWrapper<{rootTypeName}, {currentPropertyTypeName}>(instance, lens);");
            sb.AppendLine("        }");
            sb.AppendLine();

            // Generate lenses for nested properties
            if (property.Type is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.TypeKind == TypeKind.Class)
            {
                GenerateLensMethodsForNestedType(namedTypeSymbol, sb, rootTypeName, currentPropertyTypeName, currentPropertyName);
            }
        }
    }

    private static void GenerateLensMethodsForNestedType(INamedTypeSymbol typeSymbol, StringBuilder sb, string rootTypeName, string parentTypeName, string parentPropertyName)
    {
        var properties = typeSymbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(p => p.Name != "EqualityContract" && !ShouldSkipLens(p.Type))
            .ToArray();

        foreach (var property in properties)
        {
            var currentPropertyName = property.Name;
            var currentPropertyTypeName = property.Type.ToDisplayString();

            // Generate lens method for the nested type
            sb.AppendLine($"        public static LensWrapper<{rootTypeName}, {currentPropertyTypeName}> {currentPropertyName}Lens(this LensWrapper<{rootTypeName}, {parentTypeName}> wrapper)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var lens = new Lens<{parentTypeName}, {currentPropertyTypeName}>(");
            sb.AppendLine($"                part => part.{currentPropertyName},");
            sb.AppendLine($"                (whole, part) => whole with {{ {currentPropertyName} = part }});");
            sb.AppendLine($"            var composedLens = wrapper.Lens.Compose(lens);");
            sb.AppendLine($"            return new LensWrapper<{rootTypeName}, {currentPropertyTypeName}>(wrapper.Whole, composedLens);");
            sb.AppendLine("        }");
            sb.AppendLine();

            // Recurse into deeper nested properties if applicable
            if (property.Type is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.TypeKind == TypeKind.Class)
            {
                GenerateLensMethodsForNestedType(namedTypeSymbol, sb, rootTypeName, currentPropertyTypeName, currentPropertyName);
            }
        }
    }

    private static bool ShouldSkipLens(ITypeSymbol typeSymbol)
    {
        // Skip primitive types and any types from the System namespace
        if (typeSymbol is INamedTypeSymbol namedType)
        {
            if (namedType.IsValueType || namedType.SpecialType != SpecialType.None)
            {
                return true;
            }

            if (namedType.ContainingNamespace.ToDisplayString().StartsWith("System."))
            {
                return true;
            }
        }

        return false;
    }

    private class LensesSyntaxReceiver : ISyntaxReceiver
    {
        public List<TypeDeclarationSyntax> CandidateTypes { get; } = new List<TypeDeclarationSyntax>();

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
