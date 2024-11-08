using JoanComasFdz.Optics.Lenses.v1.Fluent;

namespace JoanComasFdz.Optics.Lenses.v1.SourceGenerated.IntegrationTests;

[Lenses]
public record A4(int Id, IReadOnlyCollection<B4> Bs);
public record B4(int Value);

public class TypeInNameSpaceLensesForNestedInmutableCollectionsTest
{
    [Fact]
    public void CLens_Update_ReturnsNewAWithUpdatedCPropertyOfC()
    {
        var a4 = new A4(11, [new B4(22), new B4(33)]);
        var b444 = new B4(44);

        var a4New = a4
            .BsItemLens(b4 => b4.Value == 33)
            .Update(b4 => b4 with { Value = 44 });

        //Assert.Single(a4.Bs);
        //Assert.Single(a4.Bs.Cs);

        //Assert.NotNull(a4New);
        //Assert.Single(a4.Bs);
        //Assert.Equal(2, a4New.Bs.Cs.Lenght);

        //var a4New2 = a4.Bs.ElementAt(0).CsL;
    }
}