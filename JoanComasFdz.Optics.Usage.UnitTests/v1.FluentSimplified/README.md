# Description
This folder showcases how to use Lenses for a particular domain in a fluent and simplified way.

`LibraryLensesFluentSimplified.cs` provides 3 new methods to create lenses for a `Library` to focus on single item in a collection property.

This methods no longer receive a predicate. Instead, the lens itself contains the logic about how the item is filtered / selected, and the caller only needs to specify the item id.

Those methods allow to create a `LensWrapper` for a `Library`, so that instead of first creating the `Lens` and then passing it the `Library` ike this:
```
var newLibrary = LibraryToBook("1234")
    .Compose(BookToChapter(1)
    .Update(library, chapter => chapter with { Number = 2 });
```

The lens is called directly on the `library`:
```
var newLibrary = library
    .LibraryToBook("1234")
    .BookToChapter(1)
    .With(chapter => chapter with { Number = 2});
```

> To focus on the fluent aspect, the `LibraryLensesFluentSimplified.cs` uses the existing lenses from .v1Simplified.

`CasesUsingHardcodedLensesFluentSimplified.cs` contains a set of methods with real world examples of how a developer should use the lenses.