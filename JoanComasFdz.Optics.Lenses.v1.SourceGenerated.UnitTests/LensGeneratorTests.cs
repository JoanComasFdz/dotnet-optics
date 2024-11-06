namespace JoanComasFdz.Optics.Lenses.v1.SourceGenerated.UnitTests;

public class LensGeneratorTests : SourceGeneratorBaseTest<LensSourceGenerator>
{
    [Fact]
    public Task LensesAttributeIsCreated_And_LensMethodsAreCreated()
    {
        // The source code to test
        var source = @"
using JoanComasFdz.Optics.Lenses.v1.SourceGenerated;

namespace JoanComasFdz.Optics.Lenses.v1.SourceGenerated.UnitTests;

[Lenses]
public record A(int Id, B B);
public record B(string Id, C C);
public record C(string PropertyOfC);";

        // Pass the source code to our helper and snapshot test the output
        return Verify(source);
    }
}
