using Microsoft.CodeAnalysis;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace JoanComasFdz.Optics.Lenses.v1.SourceGenerated;

internal static class TypeAnalysisTools
{
    public static bool ShouldSkipType(ITypeSymbol typeSymbol)
    {
        // Skip primitive types and any types from the System namespace
        if (typeSymbol is INamedTypeSymbol namedType)
        {
            if (namedType.IsValueType || namedType.SpecialType != SpecialType.None)
            {
                //GeneratorExecutionContext?.Info($"Skipped {namedType.ToDisplayString()}: IsValueType = {namedType.IsValueType}, SpecitalType = {namedType.SpecialType}");
                return true;
            }

            if (namedType.ContainingNamespace.ToDisplayString().StartsWith("System.") && !IsKnownCollectionType(namedType))
            {
                //GeneratorExecutionContext?.Info($"Skipped {namedType.ToDisplayString()}: Namespace = {namedType.ContainingNamespace.ToDisplayString()}");
                return true;
            }
        }

        //GeneratorExecutionContext?.Info($"Taking into account {typeSymbol.ToDisplayString()}.");
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
}

