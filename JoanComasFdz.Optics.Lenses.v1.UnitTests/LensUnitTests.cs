namespace JoanComasFdz.Optics.Lenses.v1.UnitTests;

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

            var aToBLens = new Lens<A, B>(a => a.B, (a, b) => a with { B = b});

            var result = aToBLens.Get(a);

            Assert.NotNull(result);
            Assert.Equal(a.B, result);
        }

        [Fact]
        public void Create_LambdaAToB_Set_ReturnsAWithNewB()
        {
            var a = new A(1, B: new B("b", new C("I am C")));

            var b2 = a.B with { Id = "b2" };

            var aToBLens = new Lens<A, B>(a => a.B, (a, b) => a with { B = b });
            var result = aToBLens.Set(a, b2);

            Assert.NotNull(result);
            Assert.Equal("b", a.B.Id);
            Assert.Equal("b2", result.B.Id);
        }

        [Fact]
        public void Create_LambdaAToB_Update_ReturnsAWithNewB()
        {
            var a = new A(1, B: new B("b", new C("I am C")));

            var aToBLens = new Lens<A, B>(a => a.B, (a, b) => a with { B = b });
            var result = aToBLens.Update(a, b => b with { Id = "b2" });

            Assert.NotNull(result);
            Assert.Equal("b", a.B.Id);
            Assert.Equal("b2", result.B.Id);
        }

        //[Fact]
        //public void Create_LambdaAToC_Throws()
        //{
        //    Assert.Throws<InvalidOperationException>(() => Lens2<A, C>.Create(a => a.B.C));
        //}

        //[Fact]
        //public void Create_LambdaAToPrimitive_Throws()
        //{
        //    Assert.Throws<InvalidOperationException>(() => Lens2<A, int>.Create(a => a.Id));
        //}

        [Fact]
        public void Compose_AtoBAndBToC_Get_ReturnsC()
        {
            var a = new A(1, B: new B("b", new C("I am C")));

            var aToBLens = new Lens<A, B>(a => a.B, (a, b) => a with { B = b });
            var bToCLens = new Lens<B, C>(b => b.C, (b, c) => b with { C = c });

            var aToCLens = aToBLens.Compose(bToCLens);

            var result = aToCLens.Get(a);

            Assert.NotNull(result);
            Assert.Equal(a.B.C, result);
        }

        [Fact]
        public void Compose_AtoBAndBToC_Set_ReturnsAWithBWithNewC()
        {
            var a = new A(1, B: new B("b", new C("I am C")));

            var aToBLens = new Lens<A, B>(a => a.B, (a, b) => a with { B = b });
            var bToCLens = new Lens<B, C>(b => b.C, (b, c) => b with { C = c });

            var aToCLens = aToBLens.Compose(bToCLens);

            var result = aToCLens.Set(a, new C("I am C2"));

            Assert.NotNull(result);
            Assert.Equal("I am C", a.B.C.PropertyOfC);
            Assert.Equal("I am C2", result.B.C.PropertyOfC);
        }
    }
}
