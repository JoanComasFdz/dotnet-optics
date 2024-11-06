# Use >> to compose lenses

In other languages, composing languages can be done with an operator, so instead of using the Compose method, it would look like:

```sharp
var aToCLens = AToBLens() >> BToCLens();
```

This could be achieved in C# by overriding the `>>` operator. Unfortunately, that forces the operator to only work with the given types, and an extra genercic type cannot be specified, which is required to compose lenses : `A->B + B->C = A->C`. The operator needs to know about `C`, which is not supported.

Following the Extension Everything language feature: https://github.com/dotnet/roslyn/issues/11159, composing Lenses could be done with the same operator as in F# (>>).

The implementation would look like something like this.
```csharp
public implicit extension Compose<TWhole, TPart, TSubPart> for Lens<TWhole, TPart>
{
    public static Lens<TWhole, TSubPart> operator >>(Lens<TWhole, TPart> left, Lens<TPart, TSubPart> right)
        => left.Compose(right);
}
```

Other potential options:

_Beginning with C# 11, the type of the right-hand operand of an overloaded shift operator can be any._
- https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/bitwise-and-shift-operators#shift-count-of-the-shift-operators

Unfortunately this is just a set of new overloads that only take into account numeric types.