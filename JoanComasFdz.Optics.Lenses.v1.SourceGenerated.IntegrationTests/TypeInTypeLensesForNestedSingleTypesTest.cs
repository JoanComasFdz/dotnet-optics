using JoanComasFdz.Optics.Lenses.v1.Fluent;

namespace JoanComasFdz.Optics.Lenses.v1.SourceGenerated.IntegrationTests
{
    public class TypeInTypeLensesForNestedSingleTypesTest
    {
        [Lenses]
        public record A2(int Id, B2 B2);
        public record B2(int Value);

        [Fact]
        public void B2Lens_Works()
        {
            var a2 = new A2(11, new B2(22));
            var a2New = a2.B2Lens().Update(b2 => b2 with { Value = 33 });

            Assert.NotNull(a2New);
            Assert.Equal(11, a2.Id);
            Assert.Equal(22, a2.B2.Value);

            Assert.Equal(11, a2New.Id);
            Assert.Equal(33, a2New.B2.Value);
        }
    }
}
