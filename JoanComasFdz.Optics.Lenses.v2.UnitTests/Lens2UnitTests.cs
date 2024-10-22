using JoanComasFdz.Optics.Lenses.v2;

namespace JoanComasFdz.Optics.Lenses.v2.UnitTests;

public class LensUnitTests
{
    public class NestedProperties
    {
        public record A(int Id, B B);           // Root: primitive type, complex type.
        public record B(string Id, C C);        // First level nesting. Contains: primitive type, complex type.
        public record C(string PropertyOfC);    // Second level nesting. Contains: primitive type.

        [Fact]
        public void Create_LambdaAToB_Get_ReturnsB()
        {
            var a = new A(1, B: new B("b", new C("I am C")));

            var aToBLens = Lens<A, B>.Create(a => a.B);

            var result = aToBLens.Get(a);

            Assert.NotNull(result);
            Assert.Equal(a.B, result);
        }

        [Fact]
        public void Create_LambdaAToB_Set_ReturnsAWithNewB()
        {
            var a = new A(1, B: new B("b", new C("I am C")));

            var b2 = a.B with { Id = "b2" };

            var aToBLens = Lens<A, B>.Create(a => a.B);
            var result = aToBLens.Set(a, b2);

            Assert.NotNull(result);
            Assert.Equal("b", a.B.Id);
            Assert.Equal("b2", result.B.Id);
        }

        [Fact]
        public void Create_LambdaAToB_Update_ReturnsAWithNewB()
        {
            var a = new A(1, B: new B("b", new C("I am C")));

            var aToBLens = Lens<A, B>.Create(a => a.B);
            var result = aToBLens.Update(a, b => b with { Id = "b2" });

            Assert.NotNull(result);
            Assert.Equal("b", a.B.Id);
            Assert.Equal("b2", result.B.Id);
        }

        [Fact]
        public void Create_LambdaAToC_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => Lens<A, C>.Create(a => a.B.C));
        }

        [Fact]
        public void Create_LambdaAToPrimitive_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => Lens<A, int>.Create(a => a.Id));
        }

        [Fact]
        public void Compose_AtoBAndBToC_Get_ReturnsC()
        {
            var a = new A(1, B: new B("b", new C("I am C")));

            var aToBLens = Lens<A, B>.Create(a => a.B);
            var bToCLens = Lens<B, C>.Create(b => b.C);

            var aToCLens = aToBLens.Compose(bToCLens);

            var result = aToCLens.Get(a);

            Assert.NotNull(result);
            Assert.Equal(a.B.C, result);
        }

        [Fact]
        public void Compose_AtoBAndBToC_Set_ReturnsAWithBWithNewC()
        {
            var a = new A(1, B: new B("b", new C("I am C")));

            var aToBLens = Lens<A, B>.Create(a => a.B);
            var bToCLens = Lens<B, C>.Create(b => b.C);

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
            var a = new A([new B(1, []), new B(2, [])]);
            var aToBsLens = Lens<A, IReadOnlyCollection<B>>.Create(a => a.Bs);

            var result = aToBsLens.Get(a);

            Assert.NotNull(result);
            Assert.Equal(2, a.Bs.Count);
            Assert.Equal(2, result.Count);
            Assert.Equal(a.Bs.ElementAt(0), result.ElementAt(0));
            Assert.Equal(a.Bs.ElementAt(1), result.ElementAt(1));
        }

        [Fact]
        public void Create_AToCollectionOfB_Set_ReturnsNewA()
        {
            var a = new A([new B(1, []), new B(2, [])]);
            var aToBsLens = Lens<A, IReadOnlyCollection<B>>.Create(a => a.Bs);

            var result = aToBsLens.Set(a, [new B(3, [])]);

            Assert.NotNull(result);
            Assert.Equal(2, a.Bs.Count);
            Assert.Equal(1, a.Bs.ElementAt(0).Id);
            Assert.Equal(2, a.Bs.ElementAt(1).Id);
            Assert.Single(result.Bs);
            Assert.Equal(3, result.Bs.ElementAt(0).Id);
        }

        [Fact]
        public void Create_AToCollectionOfB_Update_ReturnsNewA()
        {
            var a = new A([new B(1, []), new B(2, [])]);
            var aToBsLens = Lens<A, IReadOnlyCollection<B>>.Create(a => a.Bs);

            var result = aToBsLens.Update(a, bs => [.. bs, new B(3, [])]);

            Assert.NotNull(result);
            Assert.Equal(2, a.Bs.Count);
            Assert.Equal(1, a.Bs.ElementAt(0).Id);
            Assert.Equal(2, a.Bs.ElementAt(1).Id);
            Assert.Equal(3, result.Bs.Count);
            Assert.Equal(1, result.Bs.ElementAt(0).Id);
            Assert.Equal(2, result.Bs.ElementAt(1).Id);
            Assert.Equal(3, result.Bs.ElementAt(2).Id);
        }

        [Fact]
        public void Create_AToBWithPredicate_Get_ReturnsB()
        {
            var a = new A([new B(1, []), new B(2, [])]);
            var aToBLens = Lens<A, B>.Create<IReadOnlyCollection<B>>(a => a.Bs, b => b.Id == 1);

            var result = aToBLens.Get(a);

            Assert.NotNull(result);
            Assert.Equal(a.Bs.ElementAt(0), result);
        }

        [Fact]
        public void Create_AToBWithPredicate_Set_ReturnsAWithNewBForMtchingId()
        {
            var a = new A([new B(1, []), new B(2, [])]);
            var aToBLens = Lens<A, B>.Create<IReadOnlyCollection<B>>(a => a.Bs, b => b.Id == 1);

            var result = aToBLens.Set(a, new B(3, []));

            Assert.NotNull(result);
            Assert.Equal(2, a.Bs.Count);
            Assert.Equal(1, a.Bs.ElementAt(0).Id);
            Assert.Equal(2, a.Bs.ElementAt(1).Id);
            Assert.Equal(3, result.Bs.ElementAt(0).Id);
            Assert.Equal(2, result.Bs.ElementAt(1).Id);
        }

        [Fact]
        public void Create_AToBWithPredicate_Update_ReturnsAWithNewBForMtchingId()
        {
            var a = new A([new B(1, []), new B(2, [])]);
            var aToBLens = Lens<A, B>.Create<IReadOnlyCollection<B>>(a => a.Bs, b => b.Id == 1);

            var result = aToBLens.Update(a, book => book with { Id = 3 });

            Assert.NotNull(result);
            Assert.Equal(2, a.Bs.Count);
            Assert.Equal(1, a.Bs.ElementAt(0).Id);
            Assert.Equal(2, a.Bs.ElementAt(1).Id);
            Assert.Equal(3, result.Bs.ElementAt(0).Id);
            Assert.Equal(2, result.Bs.ElementAt(1).Id);
        }

        [Fact]
        public void Create_AToBWithPredicateAndBToCollectionOfCs_Get_ReturnsCollectionOfCs()
        {
            var a = new A([new B(1, [new C("c1")]), new B(2, [])]);
            var aToBLens = Lens<A, B>.Create<IReadOnlyCollection<B>>(a => a.Bs, b => b.Id == 1);
            var bToCsLens = Lens<B, IReadOnlyCollection<C>>.Create(b => b.Cs);

            var aToCsLens = aToBLens.Compose(bToCsLens);

            var result = aToCsLens.Get(a);

            Assert.NotNull(result);
            Assert.Equal(a.Bs.ElementAt(0).Cs.Count, result.Count);
            Assert.Equal(a.Bs.ElementAt(0).Cs.ElementAt(0), result.ElementAt(0));
        }

        [Fact]
        public void Create_AToBWithPredicateAndBToCollectionOfCs_Set_ReturnsNewA()
        {
            var a = new A([new B(1, [new C("c1")]), new B(2, [])]);
            var aToBLens = Lens<A, B>.Create<IReadOnlyCollection<B>>(a => a.Bs, b => b.Id == 1);
            var bToCsLens = Lens<B, IReadOnlyCollection<C>>.Create(b => b.Cs);

            var aToCsLens = aToBLens.Compose(bToCsLens);

            var result = aToCsLens.Set(a, [new C("c2"), new C("c3")]);

            Assert.NotNull(result);
            Assert.Single(a.Bs.ElementAt(0).Cs);
            Assert.Equal("c1", a.Bs.ElementAt(0).Cs.ElementAt(0).Id);
            Assert.Equal(2, result.Bs.ElementAt(0).Cs.Count);
            Assert.Equal("c2", result.Bs.ElementAt(0).Cs.ElementAt(0).Id);
            Assert.Equal("c3", result.Bs.ElementAt(0).Cs.ElementAt(1).Id);
        }

        [Fact]
        public void Create_AToBWithPredicateAndBToCollectionOfCs_Update_ReturnsNewA()
        {
            var a = new A([new B(1, [new C("c1")]), new B(2, [])]);
            var aToBLens = Lens<A, B>.Create<IReadOnlyCollection<B>>(a => a.Bs, b => b.Id == 1);
            var bToCsLens = Lens<B, IReadOnlyCollection<C>>.Create(b => b.Cs);

            var aToCsLens = aToBLens.Compose(bToCsLens);

            var result = aToCsLens.Update(a, cs => [.. cs, new C("c2")]);

            Assert.NotNull(result);
            Assert.Single(a.Bs.ElementAt(0).Cs);
            Assert.Equal("c1", a.Bs.ElementAt(0).Cs.ElementAt(0).Id);
            Assert.Equal(2, result.Bs.ElementAt(0).Cs.Count);
            Assert.Equal("c1", result.Bs.ElementAt(0).Cs.ElementAt(0).Id);
            Assert.Equal("c2", result.Bs.ElementAt(0).Cs.ElementAt(1).Id);
        }

        [Fact]
        public void Create_WithWrongType_LambdaWithNestedCollectionAndLambdaWithPredicate_Throws()
        {
            var a = new A([new B(1, [])]);

            Assert.Throws<LensTypeDoesNotMatchDataTypeException>(() => Lens<A, IEnumerable<B>>.Create(a => a.Bs)
            );
        }
    }
}