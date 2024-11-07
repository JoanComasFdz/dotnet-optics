using JoanComasFdz.Optics.Lenses.v1.Fluent;

namespace JoanComasFdz.Optics.Lenses.v1.SourceGenerated.IntegrationTests;

public class TypeInNamespaceLensesForNestedSingleTypesDeclaredInsideTest
{
    [Lenses]
    public record A3(int Id, A3.B3 BThree)
    {
        public record B3(int Value, B3.C3 CThree)
        {
            public record C3(int Count);
        }
    }

    [Fact]
    public void C3Lens_Works()
    {
        var a3 = new A3(11, new A3.B3(22, new A3.B3.C3(33)));
        var a3New = a3
            .BThreeLens()
            .CThreeLens()
            .Update(c3 => c3 with { Count = 44 });

        Assert.NotNull(a3New);
        Assert.Equal(11, a3.Id);
        Assert.Equal(22, a3.BThree.Value);

        Assert.Equal(11, a3New.Id);
        Assert.Equal(22, a3New.BThree.Value);
        Assert.Equal(44, a3New.BThree.CThree.Count);
    }
}
