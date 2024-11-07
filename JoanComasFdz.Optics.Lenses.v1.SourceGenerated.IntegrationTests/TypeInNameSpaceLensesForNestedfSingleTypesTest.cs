using JoanComasFdz.Optics.Lenses.v1.Fluent;

namespace JoanComasFdz.Optics.Lenses.v1.SourceGenerated.IntegrationTests;

[Lenses]
public record A1(int Id, B1 B);
public record B1(int Value, C1 C);
public record C1(int Count);

public class TypeInNameSpaceLensesForNestedfSingleTypesTest
{
    [Fact]
    public void CLens_Update_ReturnsNewAWithUpdatedCPropertyOfC()
    {
        var a1 = new A1(11, new B1(22, new C1(33)));
        var a2 = a1
            .BLens()
            .CLens()
            .Update(c1 => c1 with { Count = 55 });

        Assert.Equal(22, a1.B.Value);
        Assert.Equal(33, a1.B.C.Count);

        Assert.NotNull(a2);
        Assert.Equal(22, a2.B.Value);
        Assert.Equal(55, a2.B.C.Count);
    }
}