# Description
This folder showcases how to use Lenses for a particular domain in a fluent way.

`LibraryLensesFluent.cs` provides methods to create a `LensWrapper` for a `Library`, so that instead of first creating the `Lens` and then passing it the `Library` ike this:
```
var newLibrary = LibraryToBook(book => book.ISDN == "1234")
    .Compose(BookToChapter(chapter => chapter.Number == 1)
    .Update(library, chapter => chapter with { Number = 2 });
```

The lens is called directly on the `library`:
```
var newLibrary = library
    .BookLens(book => book.ISDN == "1234")
    .ChapterLens(chapter => chapter.Number == 1)
    .With(chapter => chapter with { Number = 2});
```

> To focus on the fluent aspect, the `LibraryLensesFluent.cs` uses the existing lenses from .v1.

`CasesUsingHardcodedLensesFluent.cs` contains a set of methods with real world examples of how a developer should use the lenses.