# dotnet-optics
This repository contains a library and an example application of how to implement and use the Functional Programming concept of Optics. It also introduces a spin in the API to enhcance readability and reduce noise when creating lenses.

The Optics library provides the basic tooling to work with lenses (for now).

The TestApp provides different ways to achieve the same thing:
- No lenses, using just c# syntax via the `with` operator: Serves as a base line to understand the problem.
- Using the lenses "as is", following the theroy: Showcases lenses.
- Using an enhanced API: Showcases a more fluent API with syntax akin to the default C# `with` operator.

## Examples

### Updating a proeprty deeply nested in a `record`

In plain C#:
```csharp
return library with
{
    Books = library.Books.Select(book => book.ISDN == bookISDN
        ? book with
        {
            Chapters = book.Chapters.Select(chapter => chapter.Number == chapterNumber
                ? chapter with
                {
                    Pages = chapter.Pages.Select(page => page.Number == pageNumber
                        ? page with
                        {
                            Content = newContent
                        }
                        : page).ToArray()
                }
                : chapter).ToArray()
        }
        : book).ToArray()
};
```

Using Lenses:
```csharp
return LibraryToBookLens(book => book.ISDN == bookISDN)
    .Compose(BookToChapterLens(chapter => chapter.Number == pageNumber))
    .Compose(ChapterToPageLens(page => page.Number == chapterNumber))
    .Mutate(library, page => page with { Content = newContent });
```

Using enhanced API:
```csharp
return library
    .BookLens(book => book.ISDN == bookISDN)
    .ChapterLens(chapter => chapter.Number == chapterNumber)
    .PageLens(page => page.Number == pageNumber)
    .With(page => page with { Content = newContent });
```

## How do Lenses work?

## How does the enchanced API work?

## Future work
- [x] Add examples to remove items from collections.
- [ ] Further enhance the API to pass only the actual item to be filtered by, like: `library.BookLens(bookISDN)`.
- [ ] Investigate simplifying lens declaration: https://stackoverflow.com/questions/68012124/record-lenses-expression-tree-for-with-expression
- [ ] Move example code to a unit testing project.
- [ ] Use Source Generators to automatically create the lenses when a `record` is marked with an attribute like `[Lenses]`, like: https://github.com/Tinkoff/Visor
- [ ] Add more methods in the enhcaned API to play with collections (`Add(T item)`, `Remove(T item)`).
