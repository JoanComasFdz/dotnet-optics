namespace JoanComasFdz.Optics.Lenses.v1.UnitTests;

public class LensUnitTests
{
    public class NestedProperties
    {
        public record A(int Id, B B);           // Root: primitive type, complex type.
        public record B(string Id, C C);        // First level nesting. Contains: primitive type, complex type.
        public record C(string PropertyOfC);    // Second level nesting. Contains: primitive type.

        [Fact]
        public void Create_LambdaAToId_Get_ReturnsId()
        {
            var a = new A(11, new B("b", new C("I am C")));

            var aToIdLens = new Lens<A, int>(source => source.Id, (source, newId) => source with { Id = newId });

            var result = aToIdLens.Get(a);

            Assert.Equal(11, result);
        }

        [Fact]
        public void Create_LambdaAToPrimitiveType_Set_ReturnsAWithNewId()
        {
            var a = new A(11, new B("b", new C("I am C")));

            var aToIdLens = new Lens<A, int>(source => source.Id, (source, newId) => source with { Id = newId });

            var result = aToIdLens.Set(a, 22);

            Assert.NotNull(result);
            Assert.Equal(11, a.Id);
            Assert.Equal(22, result.Id);
        }

        [Fact]
        public void Create_LambdaAToB_Get_ReturnsB()
        {
            var a = new A(1, B: new B("b", new C("I am C")));

            var aToBLens = new Lens<A, B>(source => source.B, (source, newB) => source with { B = newB});

            var result = aToBLens.Get(a);

            Assert.NotNull(result);
            Assert.Equal(a.B, result);
        }

        [Fact]
        public void Create_LambdaAToB_Set_ReturnsAWithNewB()
        {
            var a = new A(1, B: new B("b", new C("I am C")));

            var b2 = a.B with { Id = "b2" };

            var aToBLens = new Lens<A, B>(source => source.B, (source, newB) => source with { B = newB });
            var result = aToBLens.Set(a, b2);

            Assert.NotNull(result);
            Assert.Equal("b", a.B.Id);
            Assert.Equal("b2", result.B.Id);
        }

        [Fact]
        public void Create_LambdaAToB_Update_ReturnsAWithNewB()
        {
            var a = new A(1, B: new B("b", new C("I am C")));

            var aToBLens = new Lens<A, B>(source => source.B, (source, newB) => source with { B = newB });
            var result = aToBLens.Update(a, b => b with { Id = "b2" });

            Assert.NotNull(result);
            Assert.Equal("b", a.B.Id);
            Assert.Equal("b2", result.B.Id);
        }

        [Fact]
        public void Compose_AtoBAndBToC_Get_ReturnsC()
        {
            var a = new A(1, B: new B("b", new C("I am C")));

            var aToBLens = new Lens<A, B>(source => source.B, (source, newB) => source with { B = newB });
            var bToCLens = new Lens<B, C>(source => source.C, (source, newC) => source with { C = newC });

            var aToCLens = aToBLens.Compose(bToCLens);

            var result = aToCLens.Get(a);

            Assert.NotNull(result);
            Assert.Equal(a.B.C, result);
        }

        [Fact]
        public void Compose_AtoBAndBToC_Set_ReturnsAWithBWithNewC()
        {
            var a = new A(1, B: new B("b", new C("I am C")));

            var aToBLens = new Lens<A, B>(source => source.B, (source, newB) => source with { B = newB });
            var bToCLens = new Lens<B, C>(source => source.C, (source, newC) => source with { C = newC });

            var aToCLens = aToBLens.Compose(bToCLens);

            var result = aToCLens.Set(a, new C("I am C2"));

            Assert.NotNull(result);
            Assert.Equal("I am C", a.B.C.PropertyOfC);
            Assert.Equal("I am C2", result.B.C.PropertyOfC);
        }
    }

    public class NestedCollections
    {
        public record A(IReadOnlyCollection<B> Bs);         // A -> B returns collection. A -(predicate)-> B returns item.
        public record B(int Id, IReadOnlyCollection<C> Cs); // Firts level nesting. B -> C returns collection. B -(predicate)-> C returns item.
        public record C(string Id);                         // Item of first level nesting.

        [Fact]
        public void Create_AToCollectionOfB_Get_ReturnsCollectionOfBs()
        {
            var a = new A([new B(1, [])]);

            var aToBsLens = new Lens<A, IReadOnlyCollection<B>>(source => source.Bs, (source, newBs) => source with { Bs = newBs });

            var result = aToBsLens.Get(a);

            Assert.NotNull(result);
            Assert.Equal(a.Bs, result);
        }

        [Fact]
        public void Create_AToCollectionOfB_Set_ReturnsNewA()
        {
            var a = new A([new B(1, [])]);

            var aToBsLens = new Lens<A, IReadOnlyCollection<B>>(source => source.Bs, (source, newBs) => source with { Bs = newBs });

            var result = aToBsLens.Set(a, [new B(2, [])]);

            Assert.NotNull(result);
            Assert.Single(a.Bs);
            Assert.Equal(1, a.Bs.ElementAt(0).Id);
            Assert.Single(result.Bs);
            Assert.Equal(2, result.Bs.ElementAt(0).Id);
        }

        [Fact]
        public void Create_AToCollectionOfB_Update_ReturnsNewA()
        {
            var a = new A([new B(1, [])]);

            var aToBsLens = new Lens<A, IReadOnlyCollection<B>>(source => source.Bs, (source, newBs) => source with { Bs = newBs });

            var result = aToBsLens.Update(a, newBs => [new B(2, [])]);

            Assert.NotNull(result);
            Assert.Single(a.Bs);
            Assert.Equal(1, a.Bs.ElementAt(0).Id);
            Assert.Single(result.Bs);
            Assert.Equal(2, result.Bs.ElementAt(0).Id);
        }
    }
}
