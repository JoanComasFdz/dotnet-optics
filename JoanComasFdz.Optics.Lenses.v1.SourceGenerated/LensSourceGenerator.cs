using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace JoanComasFdz.Optics.Lenses.v1.SourceGenerated;

public static class GeneratorExecutionContextDiagnosticExtensions
{
    // Create a DiagnosticDescriptor for an informational message
    public static void Info(this GeneratorExecutionContext context, string message)
    {
        var descriptor = new DiagnosticDescriptor(
            id: "LensSourceGeneratorInfo",
            title: "Lens Source Generator Info",
            messageFormat: message,
            category: "SourceGenerator",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        var diagnostic = Diagnostic.Create(descriptor, Location.None);

        context.ReportDiagnostic(diagnostic);
    }
}

[Generator]
public class LensSourceGenerator : ISourceGenerator
{
    internal static GeneratorExecutionContext? GeneratorExecutionContext;

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForPostInitialization(ctx => ctx.AddSource("LensesAttribute.g.cs", SourceText.From(LensesAttributeCode.Attribute, Encoding.UTF8)));

        context.RegisterForSyntaxNotifications(() => new LensesSyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        Debugger.Launch();

        if (context.SyntaxReceiver is not LensesSyntaxReceiver receiver)
            return;

        GeneratorExecutionContext = context;

        foreach (var typeDeclaration in receiver.CandidateTypes)
        {
            var semanticModel = context.Compilation.GetSemanticModel(typeDeclaration.SyntaxTree);
            var typeSymbol = semanticModel.GetDeclaredSymbol(typeDeclaration) as INamedTypeSymbol;
            if (typeSymbol == null)
                continue;

            // Generate the lenses for this type
            var sourceText = GenerateLensClass(typeSymbol);

            // Create the csharp file
            context.AddSource($"{typeSymbol.Name}Lenses.g.cs", SourceText.From(sourceText, Encoding.UTF8));
        }
    }

    private static string GenerateLensClass(INamedTypeSymbol typeSymbol)
    {
        GeneratorExecutionContext?.Info($"Creating lense for {typeSymbol.ToDisplayString()}...");
        var namespaceName = typeSymbol.ContainingNamespace.ToDisplayString();
        var typeName = typeSymbol.Name;
        var fullTypeName = typeSymbol.ToDisplayString();
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

        GenerateLensMethodsForRootType(typeSymbol, sb, fullTypeName);

        sb.AppendLine("    }");

        if (namespaceName != "<global namespace>")
        {
            sb.AppendLine("}");
        }

        var lensClassContent = sb.ToString();
        GeneratorExecutionContext?.Info($"Lens class created:{lensClassContent.Replace(Environment.NewLine, string.Empty)}");
        return lensClassContent;
    }

    private static void GenerateLensMethodsForRootType(INamedTypeSymbol typeSymbol, StringBuilder sb, string rootTypeName)
    {
        GeneratorExecutionContext?.Info($"Creating lenses for {rootTypeName}...");

        var properties = typeSymbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(p => p.Name != "EqualityContract" && !ShouldSkipType(p.Type))
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
            if (property.Type is INamedTypeSymbol namedTypeSymbol)
            {
                if (namedTypeSymbol.TypeKind == TypeKind.Class)
                {
                    GenerateLensMethodsForNestedType(namedTypeSymbol, sb, rootTypeName, currentPropertyTypeName, currentPropertyName);
                }
                else if (IsKnownCollectionType(namedTypeSymbol))
                {
                    GenerateLensMethodsForCollectionType(namedTypeSymbol, sb, rootTypeName, currentPropertyTypeName, currentPropertyName);
                }
            }

        }
    }

    private static void GenerateLensMethodsForNestedType(INamedTypeSymbol typeSymbol, StringBuilder sb, string rootTypeName, string parentTypeName, string parentPropertyName)
    {
        GeneratorExecutionContext?.Info($"Creating lenses for {parentTypeName}...");

        var properties = typeSymbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(p => p.Name != "EqualityContract" && !ShouldSkipType(p.Type))
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
            if (property.Type is INamedTypeSymbol namedTypeSymbol)
            {
                if (namedTypeSymbol.TypeKind == TypeKind.Class)
                {
                    GenerateLensMethodsForNestedType(namedTypeSymbol, sb, rootTypeName, currentPropertyTypeName, currentPropertyName);
                }
                else if (IsKnownCollectionType(namedTypeSymbol))
                {
                    GenerateLensMethodsForCollectionType(namedTypeSymbol, sb, rootTypeName, currentPropertyTypeName, currentPropertyName);
                }
            }
        }
    }

    private static void GenerateLensMethodsForCollectionType(INamedTypeSymbol collectionType, StringBuilder sb, string rootTypeName, string parentTypeName, string parentPropertyName)
    {
        var itemType = collectionType.TypeArguments.FirstOrDefault();
        if (itemType == null || itemType.TypeKind != TypeKind.Class)
        {
            return; // Skip if item type is not a class or can't be determined
        }

        var itemTypeName = itemType.ToDisplayString();
        var collectionPropertyName = parentPropertyName + "Item";

        sb.AppendLine($"        public static LensWrapper<{rootTypeName}, {itemTypeName}> {collectionPropertyName}Lens(this {rootTypeName} instance, Func<{itemTypeName}, bool> predicate)");
        sb.AppendLine("        {");
        sb.AppendLine($"            var lens = new Lens<{rootTypeName}, {itemTypeName}>(");
        sb.AppendLine($"                get => instance.{parentPropertyName}.Single(predicate),");
        sb.AppendLine($"                (whole, updatedItem) => whole with {{ {parentPropertyName} = whole.{parentPropertyName}.Select(item => predicate(item) ? updatedItem : item).ToArray() }}");
        sb.AppendLine("            );");
        sb.AppendLine($"            return new LensWrapper<{rootTypeName}, {itemTypeName}>(instance, lens);");
        sb.AppendLine("        }");
        sb.AppendLine();

        // Recursively generate nested lenses for the item type
        GenerateLensMethodsForNestedType(itemType as INamedTypeSymbol, sb, rootTypeName, itemTypeName, collectionPropertyName);
    }

    private static bool ShouldSkipType(ITypeSymbol typeSymbol)
    {
        // Skip primitive types and any types from the System namespace
        if (typeSymbol is INamedTypeSymbol namedType)
        {
            if (namedType.IsValueType || namedType.SpecialType != SpecialType.None)
            {
                GeneratorExecutionContext?.Info($"Skipped {namedType.ToDisplayString()}: IsValueType = {namedType.IsValueType}, SpecitalType = {namedType.SpecialType}");
                return true;
            }

            if (namedType.ContainingNamespace.ToDisplayString().StartsWith("System.") && !IsKnownCollectionType(namedType))
            {
                GeneratorExecutionContext?.Info($"Skipped {namedType.ToDisplayString()}: Namespace = {namedType.ContainingNamespace.ToDisplayString()}");
                return true;
            }
        }

        GeneratorExecutionContext?.Info($"Taking into account {typeSymbol.ToDisplayString()}.");
        return false;
    }

    private static readonly HashSet<string> CommonCollectionTypeNames =
    [
        typeof(IEnumerable<>).FullName,
        typeof(IEnumerable).FullName,
        typeof(ICollection<>).FullName,
        typeof(ICollection).FullName,
        typeof(IList<>).FullName,
        typeof(IDictionary<,>).FullName,
        typeof(IReadOnlyCollection<>).FullName,
        typeof(IReadOnlyList<>).FullName,
        typeof(IReadOnlyDictionary<,>).FullName,
        typeof(ImmutableList<>).FullName,
        typeof(ImmutableHashSet<>).FullName,
        typeof(ImmutableDictionary<,>).FullName,
        typeof(ImmutableQueue<>).FullName,
        typeof(ImmutableStack<>).FullName,
        typeof(IImmutableList<>).FullName,
        typeof(IImmutableSet<>).FullName,
        typeof(IImmutableDictionary<,>).FullName,
        typeof(IImmutableQueue<>).FullName,
        typeof(IImmutableStack<>).FullName,
        "System.Array"  // Added to represent arrays as a collection type
    ];

    private static string NormalizeGenericTypeName(string typeName)
    {
        // Replace `1, `2, etc., with <>
        return typeName.Replace('`', '<').Replace(",", ",<>") + ">";
    }

    public static bool IsKnownCollectionType(INamedTypeSymbol namedTypeSymbol)
    {
        // TODO: Investigate further
        // Direct check, due to how type names are represented, it may not be useful:
        // For example `IReadOnlyCollection<>` vs `IReadonlyCollection<`1>`.
        if (CommonCollectionTypeNames.Contains(namedTypeSymbol.ConstructUnboundGenericType().ToString()))
        {
            return true;
        }

        // Necessary for int[], string[], etc.
        if (namedTypeSymbol.TypeKind == TypeKind.Array)
        {
            var arrayTypeSymbol = (IArrayTypeSymbol)namedTypeSymbol;
            var elementType = arrayTypeSymbol.ElementType;

            return !ShouldSkipType(elementType);
        }

        // Check if any of the interfaces implemented by the type match known collection interfaces.
        // This is necessary in case the type is a custom implementation of a collection, like:
        // - public class CustomCollection<T> : IEnumerable<T>
        // - public class MyList<T> : List<T>
        var any = namedTypeSymbol.AllInterfaces
            .Select(interfaceType => interfaceType.IsGenericType
                        ? interfaceType.ConstructUnboundGenericType().ToString()
                        : interfaceType.ToString())
            .Any(interfaceName => CommonCollectionTypeNames.Contains(interfaceName));
        return any;
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

            GeneratorExecutionContext?.Info($"Found candidate types: {string.Join(Environment.NewLine, CandidateTypes)}");
        }
    }
}
