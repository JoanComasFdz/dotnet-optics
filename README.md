# dotnet-optics
This repository contains a library and an example application of how to implement and use the Functional Programming concept of Optics. It also introduces a spin in the API to enhcance readability and reduce noise when creating lenses.

This Optics library provides the basic tooling to address this issues (for now).

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

Using hardcoded lenses:
```csharp
return LibraryToBookLens(bookISDN)
    .Compose(BookToChapterLens(pageNumber))
    .Compose(ChapterToPageLens(chapterNumber))
    .Mutate(library, page => page with { Content = newContent });
```

Using hardcoded lenses with enhanced API:
```csharp
return library
    .BookLens(bookISDN)
    .ChapterLens(chapterNumber)
    .PageLens(pageNumber)
    .With(page => page with { Content = newContent });
```

## Not-so-quick recap
A record is an inmutable data structure (that can be mutable very easily). The properties of an instance are not udpated. Instead, a new instance is created with the original values and the a new value for the desired property. C# offers the keyword `with` to do that, no extra infrastructure is needed.

Example:
```
public record Library(string Address);

var libraryDiagnoal = new Library("Diagnoal 123");
var libraryArago = libraryBcn with { Name = "Arago 88" };
```

A record may contain other records as properties. The keyowrd `with` cannot be used to specify a new value for a nested record property directly. It needs to be use at every level:
```
public record Library(Address address);
public record Address(string Street, int Number);

var libraryDiagnoal = new Library(new Address("Diagonal", 123));
var libraryArago = libraryDiagnola with { Address = library.Address with { Street = "Arago", Number = 88 }};
```
> Notice how inside the with, the assigment is done by specify `library.Address` that will be copied.

This gets verbose when there are several levels of nesting.

And it gets tricky when using collections:
```
public record Library(Address address, IReadOnlyCollection<Book> books);
public record Address(string Street, int Number);
public record Book(string ISDN, string Title);

var libraryDiagnoal = new Library(new Address("Diagonal", 123), Books: [ new Book("1234", "Dune")] );

// Add a book
var newBook = new Book("5678", "Dune 2");
var libraryWith1MoreBook = libraryDiagonal with { Books = [.. library.Books, newBook] };

// Remove a book
var bookIsdnToRemove = "1234";
var libraryWith1LessBook = libraryDiagonal with { Books = library.Books.Where(book => book.ISDN != bookIsdnToRemove).ToArray() };

// Update a book
var bookIsdnToUpdate = "5678";
var newTitle = "2 Dune 2 Furious";
var libraryWithUpdatedBook = libraryDiagonal with { Books = library.Books.Select(book => book.ISDN == bookIsdnToUpdate
    ? book with {Title = newTitle }
    : book )};
```

So:
- Adding items is done via the new array syntax: `[.. existingArray` will expand the existing array to the new one and `, newBook]` will add one item at the end of the array.
- Removing items is done copying every item except the one matching a particular condition.
- Updating an item is done via copying every item, but the one that matches a particualr condition is ignored and the edited one is returned instead.

Now if the collection is nested in another property, then it gets very noisy.

### Lens to the rescue

Functional Programming has the concept of a `Lens`. The idea is that you can focus on a particular item. Then combine lenses to reach a particular focus point.

Lenses are not used to focus on simple properties (`string`, `int`, etc.) since that can be done directly via de `with` keyword.

A `Lens` is used to focus on a property that is a complex data structure, or collections.

A `Lens` only focuses con 1 nested property. To go deeper, several lenses are combined together.

A `Lens` is a very basic structure that provides two functions:
- How to get an item.
- How to set the item.

The concept comes with an additional couple of methods (in C# implmented as extension methods):
- `Compose()`: To chain multiple lenses to go deeper.
- `Update()`: To automate looking for the item to be updated (using the `Lens.Get(...)` method, then calling `Lens.Set(...)`.).

So as a developer, you hardcode each individual lenses to navigate through nested proeprties.

Then, whenever you need to update a nested property (let's say in a RUD operation), you use the existing lenses and compose them to reach the property to be updated.

### Rules extraction:
- Non-complex proeprties do not need lenses, they are edited directly via the `with` keyword.
- Lenses only go 1 level deep.
- Lenses can return an item from a collection property.

## How do Lenses work?

## How does the enchanced API work?

## Future work
- [x] Add examples to remove items from collections.
- [x] Further enhance the API to pass only the actual item to be filtered by, like: `library.BookLens(bookISDN)`.
- [x] Investigate simplifying lens declaration: https://stackoverflow.com/questions/68012124/record-lenses-expression-tree-for-with-expression
- [x] Add a unit testing project.
- [ ] Use Source Generators to automatically create the lenses when a `record` is marked with an attribute like `[Lenses]`, like: https://github.com/Tinkoff/Visor
- [ ] Add more methods in the enhcaned API to play with collections (`Add(T item)`, `Remove(T item)`).
