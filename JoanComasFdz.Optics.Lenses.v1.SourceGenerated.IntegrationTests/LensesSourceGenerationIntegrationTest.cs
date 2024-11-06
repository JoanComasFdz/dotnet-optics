using JoanComasFdz.Optics.Lenses.v1.Fluent;

namespace JoanComasFdz.Optics.Lenses.v1.SourceGenerated.IntegrationTests
{
    [Lenses]
    public record A(int Id, B B);
    public record B(string Id, C C);
    public record C(string PropertyOfC, D D);
    public record D(decimal Id);

    public class LensesSourceGenerationIntegrationTest
    {
        [Fact]
        public void BLens_Works()
        {
            var a1 = new A(11, new B("22", new C("I am C", new D(99))));
            var a2 = a1.BLens().Update(b => b with { Id = "33" });

            Assert.NotNull(a2);
            Assert.Equal("22", a1.B.Id);
            Assert.Equal("33", a2.B.Id);
        }

        [Fact]
        public void CLens_Works()
        {
            var a1 = new A(11, new B("22", new C("I am C", new D(99))));
            var a2 = a1
                .BLens()
                .CLens()
                .Update(b => b with { PropertyOfC = "CCC" });

            Assert.Equal("22", a1.B.Id);
            Assert.Equal("I am C", a1.B.C.PropertyOfC);

            Assert.NotNull(a2);
            Assert.Equal("22", a2.B.Id);
            Assert.Equal("CCC", a2.B.C.PropertyOfC);
        }

        [Fact]
        public void DLens_Works()
        {
            var a1 = new A(11, new B("22", new C("I am C", new D(99))));
            var a2 = a1
                .BLens()
                .CLens()
                .DLens()
                .Update(d => d with { Id = 88 });

            Assert.Equal("22", a1.B.Id);
            Assert.Equal("I am C", a1.B.C.PropertyOfC);
            Assert.Equal(99, a1.B.C.D.Id);

            Assert.NotNull(a2);
            Assert.Equal("22", a2.B.Id);
            Assert.Equal("I am C", a2.B.C.PropertyOfC);
            Assert.Equal(88, a2.B.C.D.Id);
        }
    }
}