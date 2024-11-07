using JoanComasFdz.Optics.Lenses.v1.Fluent;

namespace JoanComasFdz.Optics.Lenses.v1.SourceGenerated.IntegrationTests;

[Lenses]
public record A(int Id, B B);
public record B(int Value, C C);
public record C(int Count, D D);
public record D(int Counter);

public class TypeInNameSpace_LensesForNestedfSingleTypes_Test
{
    [Fact]
    public void BLens__Update_ReturnsNewAWithUpdatedBId()
    {
        var a1 = new A(11, new B(22, new C(33, new D(44))));
        var a2 = a1.BLens().Update(b => b with { Value = 55 });

        Assert.NotNull(a2);
        Assert.Equal(22, a1.B.Value);
        Assert.Equal(55, a2.B.Value);
    }

    [Fact]
    public void CLens_Update_ReturnsNewAWithUpdatedCPropertyOfC()
    {
        var a1 = new A(11, new B(22, new C(33, new D(44))));
        var a2 = a1
            .BLens()
            .CLens()
            .Update(b => b with { Count = 55 });

        Assert.Equal(22, a1.B.Value);
        Assert.Equal(33, a1.B.C.Count);

        Assert.NotNull(a2);
        Assert.Equal(22, a2.B.Value);
        Assert.Equal(55, a2.B.C.Count);
    }

    [Fact]
    public void DLens_Update_ReturnsNewAWithUpdatedDId()
    {
        var a1 = new A(11, new B(22, new C(33, new D(44))));
        var a2 = a1
            .BLens()
            .CLens()
            .DLens()
            .Update(d => d with { Counter = 55 });

        Assert.Equal(22, a1.B.Value);
        Assert.Equal(33, a1.B.C.Count);
        Assert.Equal(44, a1.B.C.D.Counter);

        Assert.NotNull(a2);
        Assert.Equal(22, a2.B.Value);
        Assert.Equal(33, a2.B.C.Count);
        Assert.Equal(55, a2.B.C.D.Counter);
    }
}