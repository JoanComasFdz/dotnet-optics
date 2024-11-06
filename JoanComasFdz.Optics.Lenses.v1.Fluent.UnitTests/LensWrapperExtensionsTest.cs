namespace JoanComasFdz.Optics.Lenses.v1.Fluent.UnitTests;

public class LensWrapperExtensionsTest
{
    public record A(int Id, B B);           // Root: primitive type, complex type.
    public record B(string Id, C C);        // First level nesting. Contains: primitive type, complex type.
    public record C(string PropertyOfC);    // Second level nesting. Contains: primitive type.

    [Fact]
    public void Wrapper_InstanceAndLens_WithbHasNewId_ReturnsNewAWithNewBId()
    {
        var a = new A(11, new B("22", new C("I am C")));
        var aToBLens = new Lens<A, B>(
            Get: source => source.B,
            Set: (source, newB) => source with { B = newB }
            );

        var wrapper = new LensWrapper<A, B>(a, aToBLens);

        var result = wrapper.With(b => b with { Id = "33" });

        Assert.NotNull(result);
        Assert.Equal(11, result.Id);
        Assert.NotNull(result.B);
        Assert.Equal("33", result.B.Id);
        Assert.NotNull(result.B.C);
        Assert.Equal(a.B.C, result.B.C);
    }
}