using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
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
public partial class LensSourceGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForPostInitialization(ctx => ctx.AddSource("LensesAttribute.g.cs", SourceText.From(LensesAttributeCode.Attribute, Encoding.UTF8)));

        context.RegisterForSyntaxNotifications(() => new LensesSyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        //Debugger.Launch();

        if (context.SyntaxReceiver is not LensesSyntaxReceiver receiver)
            return;

        foreach (var typeDeclaration in receiver.CandidateTypes)
        {
            var semanticModel = context.Compilation.GetSemanticModel(typeDeclaration.SyntaxTree);
            var rootTypeSymbol = semanticModel.GetDeclaredSymbol(typeDeclaration) as INamedTypeSymbol;
            if (rootTypeSymbol == null)
                continue;

            // Generate the lenses for this type
            var sourceText = GenerateLensClass(rootTypeSymbol);

            // Create the csharp file
            context.AddSource($"{rootTypeSymbol.Name}Lenses.g.cs", SourceText.From(sourceText, Encoding.UTF8));
        }
    }

    private static string GenerateLensClass(INamedTypeSymbol rootTypeSymbol)
    {
        var namespaceName = rootTypeSymbol.ContainingNamespace.ToDisplayString();
        var typeName = rootTypeSymbol.Name;
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

        RootTypePropertiesLensGenerator.GenerateLensMethodsForRootType(sb, rootTypeSymbol);

        sb.AppendLine("    }");

        if (namespaceName != "<global namespace>")
        {
            sb.AppendLine("}");
        }

        var lensClassContent = sb.ToString();
        return lensClassContent;
    }
}
