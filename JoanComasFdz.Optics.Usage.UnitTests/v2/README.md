# Description
This folder showcases how to use second version of Lenses for a particular domain.

`LibraryLenses.cs` provides the methods necessary to create v2 lenses for a `Library`.

For collection properties, there are two sets of methods:
- Focus on the collection itself (`LibraryToBooksLens`, notice that _Books_ is plural): this allows to create a new instance of `Library` with changes in the focused collection (add, remove, filter, etc).
- Focus on an item inside the collection (`LibraryToBookLens`, notice that _Book_ is singular): this allows to create a new instance of `Library` with changes in one specific item.

For the second set of methods, they just receive the value identifying an item, so that the caller can specify the value by which the item is filtered / selected. This simplifies the API of the lens, since users no longer have to write lambdas all over the code.

`CasesUsingHardcodedLenses.cs` contains a set of methods with real world examples of how a developer should use the lenses.

The improvement here is a lot less code to create a new lens, so that instead of this:
```csharp
public static Lens<Library, IReadOnlyList<Book>> LibraryToBooksLens() => new(
    library => library.Books,
    (library, newBooks) => library with { Books = newBooks }
);

public static Lens<Library, Book> LibraryToBookLens(Func<Book, bool> predicate)
{
    return new Lens<Library, Book>(
        library => library.Books.Single(predicate),
        (library, updatedBook) => library with { Books = library.Books.Select(book => predicate(book) ? updatedBook : book).ToArray() }
    );
}
```

Lenses can be written like this:
```csharp
public static Lens<Library, IReadOnlyList<Book>> LibraryToBooksLens()
    => Lens<Library, IReadOnlyList<Book>>.Create(library => library.Books);

public static Lens<Library, Book> LibraryToBookLens(string bookISDN)
    => Lens<Library, Book>.Create<IReadOnlyCollection<Book>>(library => library.Books, book => book.ISDN == bookISDN);
```

The infrastructure creates the lenses by analyzing the expression and creating all the necessary wiring transparently. 
For collection, a second expression is used as a predicate when returning a single item.

This allows the user of the lens just has to use the values used by the predicate:

```csharp
var newLibrary = LibraryToBookLens("1234")  // Only the book ISDN is passed
            .Compose(BookToChapterLens(1))  // Only the id of the chapter is passed
            .Compose(ChapterToPageLens(2))  // Only the id of the page is passed
            .Update(library, pages => [.. pages, new Page(2, "Page 2 Content")]);
```