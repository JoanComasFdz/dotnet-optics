//HintName: ALenses.g.cs
using JoanComasFdz.Optics.Lenses.v1.Fluent;

namespace JoanComasFdz.Optics.Lenses.v1.SourceGenerated.UnitTests
{
    public static class ALenses
    {
        public static LensWrapper<A, JoanComasFdz.Optics.Lenses.v1.SourceGenerated.UnitTests.B> BLens(this A instance)
        {
            var lens = new Lens<A, JoanComasFdz.Optics.Lenses.v1.SourceGenerated.UnitTests.B>(
                get => instance.B,
                (whole, part) => whole with { B = part });
            return new LensWrapper<A, JoanComasFdz.Optics.Lenses.v1.SourceGenerated.UnitTests.B>(instance, lens);
        }

        public static LensWrapper<A, JoanComasFdz.Optics.Lenses.v1.SourceGenerated.UnitTests.C> CLens(this LensWrapper<A, JoanComasFdz.Optics.Lenses.v1.SourceGenerated.UnitTests.B> wrapper)
        {
            var lens = new Lens<JoanComasFdz.Optics.Lenses.v1.SourceGenerated.UnitTests.B, JoanComasFdz.Optics.Lenses.v1.SourceGenerated.UnitTests.C>(
                part => part.C,
                (whole, part) => whole with { C = part });
            var composedLens = wrapper.Lens.Compose(lens);
            return new LensWrapper<A, JoanComasFdz.Optics.Lenses.v1.SourceGenerated.UnitTests.C>(wrapper.Whole, composedLens);
        }

    }
}
