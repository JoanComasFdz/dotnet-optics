using Microsoft.CodeAnalysis;
using System.Linq;
using System.Text;

namespace JoanComasFdz.Optics.Lenses.v1.SourceGenerated;

/// <summary>
/// A ----> B -----> C
/// ^       ^        ^
/// root    current  child (property)
///
/// Lens: B -> C
/// LensWrapper: A -> C
///
/// C may or may not be a collection.
/// </summary>
public static class NestedTypePropertiesLensGenerator
{
    public static void GenerateLensMethodsForNestedType(
        StringBuilder sb,
        string rootTypeFullName,                    // A
        INamedTypeSymbol currentNamedTypeSymbol,    // B
        string currentPropertyName)                 // "Bs"
    {
        var currentTypeFullName = currentNamedTypeSymbol.ToDisplayString();
        var properties = currentNamedTypeSymbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(p => p.Name != "EqualityContract" && !TypeAnalysisTools.ShouldSkipType(p.Type))
            .ToArray();

        foreach (var property in properties)
        {
            var childPropertyName = property.Name;
            var childTypeFullName = property.Type.ToDisplayString();

            // Generate lens method for the nested type
            sb.AppendLine($"        public static LensWrapper<{rootTypeFullName}, {childTypeFullName}> {childPropertyName}Lens(this LensWrapper<{rootTypeFullName}, {currentTypeFullName}> wrapper)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var lens = new Lens<{currentTypeFullName}, {childTypeFullName}>(");
            sb.AppendLine($"                part => part.{childPropertyName},");
            sb.AppendLine($"                (whole, part) => whole with {{ {childPropertyName} = part }});");
            sb.AppendLine($"            var composedLens = wrapper.Lens.Compose(lens);");
            sb.AppendLine($"            return new LensWrapper<{rootTypeFullName}, {childTypeFullName}>(wrapper.Whole, composedLens);");
            sb.AppendLine("        }");
            sb.AppendLine();

            if (property.Type is not INamedTypeSymbol childNamedTypeSymbol)
            {
                continue;
            }

            // Generate lenses for nested type
            if (childNamedTypeSymbol.TypeKind == TypeKind.Class)
            {
                GenerateLensMethodsForNestedType(sb, rootTypeFullName, childNamedTypeSymbol, childPropertyName);
            }
            else if (TypeAnalysisTools.IsKnownCollectionType(childNamedTypeSymbol))
            {
                GenerateLensMethodsForSingleItemInCollectionForNestedType(sb, rootTypeFullName, currentTypeFullName, childNamedTypeSymbol, childPropertyName);
            }
        }
    }

    /// <summary>
    /// A ----> B -----> [C]
    /// ^       ^        ^
    /// root    current  child collection (property) of item C
    ///
    /// Lens B -> C         // From Parent to an item in the collection
    /// LensWrapper A -> C
    ///
    /// </summary>
    private static void GenerateLensMethodsForSingleItemInCollectionForNestedType(
        StringBuilder sb,
        string rootTypeFullName,                            // A
        string currentTypeName,                             // B
        INamedTypeSymbol childCollectionNamedTypeSymbol,    // [C]
        string childCollectionPropertyName                  // "Cs"
        )
    {
        var itemType = childCollectionNamedTypeSymbol.TypeArguments.FirstOrDefault();
        if (itemType == null || itemType.TypeKind != TypeKind.Class)
        {
            return; // Skip if item type is not a class or can't be determined
        }

        var childCollectionTypeFullName = childCollectionNamedTypeSymbol.ToDisplayString(); // namespace.[C]
        var itemTypeFullName = itemType.ToDisplayString();

        sb.AppendLine($"        public static LensWrapper<{rootTypeFullName}, {itemTypeFullName}> {childCollectionPropertyName}Lens(this LensWrapper<{rootTypeFullName}, {currentTypeName}> wrapper, Func<{itemTypeFullName}, bool> predicate)");
        sb.AppendLine("        {");
        sb.AppendLine($"            var lens = new Lens<{currentTypeName}, {itemTypeFullName}>(");
        sb.AppendLine($"                get => get.{childCollectionPropertyName}.Single(predicate),");
        sb.AppendLine($"                (whole, updatedItem) => whole with {{ {childCollectionPropertyName} = whole.{childCollectionPropertyName}.Select(item => predicate(item) ? updatedItem : item).ToArray()  }}");
        sb.AppendLine("            );");
        sb.AppendLine($"            var composedLens = wrapper.Lens.Compose(lens);");
        sb.AppendLine($"            return new LensWrapper<{rootTypeFullName}, {itemTypeFullName}>(wrapper.Whole, composedLens);");
        sb.AppendLine("        }");
        sb.AppendLine();

        if (itemType is not INamedTypeSymbol itemTypeNamedTypeSymbol)
        {
            return;
        }

        // Recursively generate nested lenses for the item type
        GenerateLensMethodsForNestedType(sb, rootTypeFullName, itemTypeNamedTypeSymbol, itemTypeFullName);
    }

}