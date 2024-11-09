using JoanComasFdz.Optics.Lenses.v1.Fluent;

namespace JoanComasFdz.Optics.Lenses.v1.SourceGenerated.IntegrationTests;

[Lenses]
public record A4(int Id, IReadOnlyCollection<B4> Bs);
public record B4(int Value, IReadOnlyCollection<C4> Cs);
public record C4(int Count);

public class TypeInNameSpaceLensesForNestedInmutableCollectionsTest
{
    [Fact]
    public void CLens_Update_ReturnsNewAWithUpdatedCPropertyOfC()
    {
        var a4 = new A4(11, [new B4(22, [new C4(222)]), new B4(33, [new C4(333)])]);

        var a4New = a4
            .BsLens(b4 => b4.Value == 33)
            .CsLens(c4 => c4.Count == 333)
            .Update(c4 => c4 with { Count = 444 });

        Assert.NotNull(a4New);
        Assert.NotNull(a4New);
        Assert.Equal(2, a4New.Bs.Count);
        Assert.Single(a4New.Bs.ElementAt(1).Cs);
        Assert.Equal(444, a4New.Bs.ElementAt(1).Cs.ElementAt(0).Count);
    }
}