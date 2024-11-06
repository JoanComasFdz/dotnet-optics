using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
namespace JoanComasFdz.Optics.Lenses.v1.SourceGenerated.UnitTests;

/// <summary>
/// From: https://andrewlock.net/creating-a-source-generator-part-2-testing-an-incremental-generator-with-snapshot-testing/
/// </summary>
/// <typeparam name="TSourceGenerator"></typeparam>
public abstract class SourceGeneratorBaseTest<TSourceGenerator> where TSourceGenerator : class, ISourceGenerator
{
    protected static Task Verify(string source)
    {
        // Parse the provided string into a C# syntax tree
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

        // Create references for assemblies we require
        // We could add multiple references if required
        IEnumerable<PortableExecutableReference> references =
        [
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
        ];

        // Create a Roslyn compilation for the syntax tree.
        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: typeof(SourceGeneratorBaseTest<TSourceGenerator>).Assembly.FullName,
            syntaxTrees: [syntaxTree],
            references: references);

        // Create an instance of our EnumGenerator incremental source generator
        var generator = Activator.CreateInstance<TSourceGenerator>();

        // The GeneratorDriver is used to run our generator against a compilation
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        // Run the source generator!
        driver = driver.RunGenerators(compilation);

        // Use verify to snapshot test the source generator output!
        return Verifier.Verify(driver);
    }
}