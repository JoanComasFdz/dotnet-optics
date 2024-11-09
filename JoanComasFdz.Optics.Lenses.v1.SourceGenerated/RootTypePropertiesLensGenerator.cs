using Microsoft.CodeAnalysis;
using System.Linq;
using System.Text;

namespace JoanComasFdz.Optics.Lenses.v1.SourceGenerated;

internal static class RootTypePropertiesLensGenerator
{
    /// <summary>
    /// A ----> B
    /// ^       ^
    /// root    child
    ///
    /// </summary>
    /// <param name="sb"></param>
    /// <param name="rootTypeSymbol"></param>
    public static void GenerateLensMethodsForRootType(
        StringBuilder sb,
        INamedTypeSymbol rootTypeSymbol  // A
        )
    {
        var rootTypeFullName = rootTypeSymbol.ToDisplayString();
        var properties = rootTypeSymbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(p => p.Name != "EqualityContract" && !TypeAnalysisTools.ShouldSkipType(p.Type))
            .ToArray();

        foreach (var property in properties)
        {
            var childTypePropertyName = property.Name;                  // "Bs"
            var childTypeFullName = property.Type.ToDisplayString();    // B

            // Generate lens method for the root type
            sb.AppendLine($"        public static LensWrapper<{rootTypeFullName}, {childTypeFullName}> {childTypePropertyName}Lens(this {rootTypeFullName} instance)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var lens = new Lens<{rootTypeFullName}, {childTypeFullName}>(");
            sb.AppendLine($"                get => instance.{childTypePropertyName},");
            sb.AppendLine($"                (whole, part) => whole with {{ {childTypePropertyName} = part }});");
            sb.AppendLine($"            return new LensWrapper<{rootTypeFullName}, {childTypeFullName}>(instance, lens);");
            sb.AppendLine("        }");
            sb.AppendLine();

            if (property.Type is not INamedTypeSymbol childNamedTypeSymbol)
            {
                continue;
            }

            // Generate lenses for nested type
            if (childNamedTypeSymbol.TypeKind == TypeKind.Class)
            {
                NestedTypePropertiesLensGenerator.GenerateLensMethodsForNestedType(sb, rootTypeFullName, childNamedTypeSymbol, childTypePropertyName);
            }
            else if (TypeAnalysisTools.IsKnownCollectionType(childNamedTypeSymbol))
            {
                GenerateLensMethodsForSingleItemInCollectionForRoot(sb, rootTypeFullName, childNamedTypeSymbol, childTypePropertyName);
            }
        }
    }

    /// <summary>
    /// A ----> [B]
    /// ^       ^
    /// root    child collection (property) of item B
    ///
    /// Lens: A -> B            // From Parent to an item in the collection
    /// LensWrapper: A -> B
    ///
    /// </summary>
    /// <param name="sb"></param>
    /// <param name="rootTypeFullName"></param>
    /// <param name="childCollectionNamedTypeSymbol"></param>
    /// <param name="childCollectionPropertyName"></param>
    private static void GenerateLensMethodsForSingleItemInCollectionForRoot(
        StringBuilder sb,
        string rootTypeFullName,                            // A
        INamedTypeSymbol childCollectionNamedTypeSymbol,    // [B]
        string childCollectionPropertyName                  // "Bs"
        )
    {
        var itemType = childCollectionNamedTypeSymbol.TypeArguments.FirstOrDefault();
        if (itemType == null || itemType.TypeKind != TypeKind.Class)
        {
            return; // Skip if item type is not a class or can't be determined
        }

        var itemTypeName = itemType.ToDisplayString();

        sb.AppendLine($"        public static LensWrapper<{rootTypeFullName}, {itemTypeName}> {childCollectionPropertyName}Lens(this {rootTypeFullName} instance, Func<{itemTypeName}, bool> predicate)");
        sb.AppendLine("        {");
        sb.AppendLine($"            var lens = new Lens<{rootTypeFullName}, {itemTypeName}>(");
        sb.AppendLine($"                get => instance.{childCollectionPropertyName}.Single(predicate),");
        sb.AppendLine($"                (whole, updatedItem) => whole with {{ {childCollectionPropertyName} = whole.{childCollectionPropertyName}.Select(item => predicate(item) ? updatedItem : item).ToArray() }}");
        sb.AppendLine("            );");
        sb.AppendLine($"            return new LensWrapper<{rootTypeFullName}, {itemTypeName}>(instance, lens);");
        sb.AppendLine("        }");
        sb.AppendLine();

        if (itemType is not INamedTypeSymbol itemTypeNamedTypeSymbol)
        {
            return;
        }

        // Recursively generate nested lenses for the item type
        NestedTypePropertiesLensGenerator.GenerateLensMethodsForNestedType(sb, rootTypeFullName, itemTypeNamedTypeSymbol, childCollectionPropertyName);
    }
}
