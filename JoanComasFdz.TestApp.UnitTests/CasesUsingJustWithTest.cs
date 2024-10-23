using JoanComasFdz.Optics.TestApp.Domain;

namespace JoanComasFdz.TestApp.UnitTests;

public class CasesUsingJustWithTest
{
    private readonly Library library = new(
        Name: "Downtown Public Library",
        Address: new Address(
            Street: "456 Oak Street",
            City: "Metropolis",
            PostalCode: "67890"
        ),
        Books: [
            new Book(
                ISDN: "1234",
                Title: "The Art of Programming",
                Author: "Alice Smith",
                Chapters: [
                    new Chapter(
                        Number: 1,
                        Title: "Introduction to Algorithms",
                        Pages: [
                            new Page(1, "Page 1 Content")
                        ]
                    )]
            )
        ]
    );

    [Fact]
    public void AddBookToLibrary_NewLibraryHasNewBook()
    {
        var secondBook = new Book(
            ISDN: "5678",
            Title: "Advanced Mathematics",
            Author: "Bob Johnson",
            Chapters: [
                new Chapter(
                    Number: 1,
                    Title: "Linear Algebra",
                    Pages: [
                        new Page(1, "Linear Algebra Content")
                    ]
                )
            ]
        );

        var newLibrary = library with
        {
            Books = [.. library.Books, secondBook]
        };

        Assert.Equal(2, newLibrary.Books.Count);
        Assert.Equal("5678", newLibrary.Books.Last().ISDN);
    }

    [Fact]
    public void AddChapterToBook_BookHasNewChapter()
    {
        var bookISDN = "1234";
        var secondChapter = new Chapter(
            Number: 2,
            Title: "First Algorithms",
            Pages: [
                new Page(10, "Page 10 Content")
            ]
        );

        // EITHER
        var bookToUpdate = library.Books.Single(b => b.ISDN == bookISDN);
        var updatedBookWithNewChapter = bookToUpdate with
        {
            Chapters = [.. bookToUpdate.Chapters, secondChapter]
        };
        var newLibrary = library with
        {
            Books = library.Books.Select(book => book.ISDN == bookISDN ? updatedBookWithNewChapter : book).ToArray()
        };

        // OR
        newLibrary = library with
        {
            Books = library.Books.Select(book => book.ISDN == bookISDN
                ? book with
                {
                    Chapters = [.. book.Chapters, secondChapter]
                }
                : book).ToArray()
        };

        var updatedBook = newLibrary.Books.Single(b => b.ISDN == bookISDN);
        Assert.Equal(2, updatedBook.Chapters.Count);
        Assert.Equal("First Algorithms", updatedBook.Chapters.Last().Title);
    }

    [Fact]
    public void AddPageToChapterOfBook_ChapterHasNewPage()
    {
        var bookISDN = "1234";
        var chapterNumber = 1;
        var secondPage = new Page(2, "Page 2 Content");

        // EITHER
        var bookToUpdate = library.Books.Single(b => b.ISDN == bookISDN);
        var chapterToUpdate = bookToUpdate.Chapters.Single(chapter => chapter.Number == chapterNumber);
        var upatedChapter = chapterToUpdate with
        {
            Pages = [.. chapterToUpdate.Pages, secondPage]
        };
        var updatedBookWithUpdatedChapter = bookToUpdate with
        {
            Chapters = bookToUpdate.Chapters.Select(chapter => chapter.Number == chapterNumber ? upatedChapter : chapter).ToArray()
        };
        var newLibrary = library with
        {
            Books = library.Books.Select(book => book.ISDN == bookISDN ? updatedBookWithUpdatedChapter : book).ToArray()
        };

        // OR
        newLibrary = library with
        {
            Books = library.Books.Select(book => book.ISDN == bookISDN
                ? book with
                {
                    Chapters = book.Chapters.Select(chapter => chapter.Number == chapterNumber
                        ? chapter with
                        {
                            Pages = [.. chapter.Pages, secondPage]
                        }
                        : chapter).ToArray()
                }
                : book).ToArray()
        };

        var updatedBook = newLibrary.Books.Single(b => b.ISDN == bookISDN);
        var updatedChapter = updatedBook.Chapters.Single(c => c.Number == chapterNumber);
        Assert.Equal(2, updatedChapter.Pages.Count);
        Assert.Equal("Page 2 Content", updatedChapter.Pages.Last().Content);
    }

    [Fact]
    public void UpdateBookTitle_BookTitleIsUpdated()
    {
        var bookISDN = "1234";
        var newBookTitle = "Program nicely";

        // EITHER
        var bookToUpdate = library.Books.Single(book => book.ISDN == bookISDN);
        var updatedBook = bookToUpdate with
        {
            Title = newBookTitle,
        };
        var newLibrary = library with
        {
            Books = library.Books.Select(book => book.ISDN == bookISDN ? updatedBook : book).ToArray()
        };

        // OR
        newLibrary = library with
        {
            Books = library.Books.Select(book => book.ISDN == bookISDN
                                            ? book with
                                            {
                                                Title = newBookTitle
                                            }
                                            : book)
                                        .ToArray()
        };

        var newLibraryBook = newLibrary.Books.Single(b => b.ISDN == bookISDN);
        Assert.Equal(newBookTitle, newLibraryBook.Title);
    }

    [Fact]
    public void UpdateChapterTitleOfBook_ChapterTitleIsUpdated()
    {
        var bookISDN = "1234";
        var chapterNumber = 1;
        var newchapterTitle = "Advanced Algorithms";

        // EITHER
        var bookToUpdate = library.Books.Single(book => book.ISDN == bookISDN);
        var chapterToUpdate = bookToUpdate.Chapters.Single(chapter => chapter.Number == chapterNumber);
        var updatedChapter = chapterToUpdate with
        {
            Title = newchapterTitle
        };
        var bookWithUpdatedChapter = bookToUpdate with
        {
            Chapters = bookToUpdate.Chapters.Select(chapter => chapter.Number == chapterNumber ? updatedChapter : chapter).ToArray()
        };
        var updatedLibrary = library with
        {
            Books = library.Books.Select(book => book.ISDN == bookISDN ? bookWithUpdatedChapter : book).ToArray()
        };

        // OR
        updatedLibrary = library with
        {
            Books = library.Books.Select(book => book.ISDN == bookISDN
                ? book with
                {
                    Chapters = book.Chapters.Select(chapter => chapter.Number == chapterNumber
                        ? chapter with
                        {
                            Title = newchapterTitle
                        }
                        : chapter).ToArray()
                }
                : book).ToArray()
        };

        var newLibraryBook = updatedLibrary.Books.Single(b => b.ISDN == bookISDN);
        var newLibraryChapter = newLibraryBook.Chapters.Single(c => c.Number == chapterNumber);
        Assert.Equal(newchapterTitle, newLibraryChapter.Title);
    }

    [Fact]
    public void UpdatePageContentOfChapterOfBook_PageContentIsUpdated()
    {
        var bookISDN = "1234";
        var chapterNumber = 1;
        var pageNumber = 1;
        var newContent = "Updated Content";

        // EITHER
        var bookToUpdate = library.Books.Single(book => book.ISDN == bookISDN);
        var chapterToUpdate = bookToUpdate.Chapters.Single(chapter => chapter.Number == chapterNumber);
        var pageToUpdate = chapterToUpdate.Pages.Single(page => page.Number == pageNumber);
        var updatedPage = pageToUpdate with
        {
            Content = newContent
        };
        var updatedChapter = chapterToUpdate with
        {
            Pages = chapterToUpdate.Pages.Select(page => page.Number == pageNumber ? updatedPage : page).ToArray()
        };
        var bookWithUpdatedChapter = bookToUpdate with
        {
            Chapters = bookToUpdate.Chapters.Select(chapter => chapter.Number == chapterNumber ? updatedChapter : chapter).ToArray()
        };
        var updatedLibrary = library with
        {
            Books = library.Books.Select(book => book.ISDN == bookISDN ? bookWithUpdatedChapter : book).ToArray()
        };

        // OR
        updatedLibrary = library with
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

        var newLibraryBook = updatedLibrary.Books.Single(b => b.ISDN == bookISDN);
        var newLibraryChapter = newLibraryBook.Chapters.Single(c => c.Number == 1);
        var newLibraryPage = newLibraryChapter.Pages.Single(p => p.Number == 1);
        Assert.Equal(newContent, newLibraryPage.Content);
    }

    [Fact]
    public void RemoveBookFromLibrary_BookIsRemoved()
    {
        var bookISDN = "1234";
        var newLibrary = library with
        {
            Books = library.Books.Where(book => book.ISDN != bookISDN).ToArray()
        };

        Assert.Empty(newLibrary.Books);
    }

    [Fact]
    public void RemoveChapterFromBook_ChapterIsRemoved()
    {
        var bookISDN = "1234";
        var chapterNumber = 1;

        // EITHER
        var bookToUpdate = library.Books.Single(b => b.ISDN == bookISDN);
        var updatedBook = bookToUpdate with
        {
            Chapters = bookToUpdate.Chapters.Where(chapter => chapter.Number != chapterNumber).ToArray()
        };
        var updatedLibrary = library with
        {
            Books = library.Books.Select(book => book.ISDN == bookISDN ? updatedBook : book).ToArray()
        };

        // OR
        updatedLibrary = library with
        {
            Books = library.Books.Select(book => book.ISDN == bookISDN
                ? book with
                {
                    Chapters = book.Chapters.Where(chapter => chapter.Number != chapterNumber).ToArray()
                }
                : book).ToArray()
        };

        var newLibraryBook = updatedLibrary.Books.Single(b => b.ISDN == bookISDN);
        Assert.Empty(newLibraryBook.Chapters);
    }

    [Fact]
    public void RemovePageFromChapterOfBook_PageIsRemoved()
    {
        var bookISDN = "1234";
        var chapterNumber = 1;
        int pageNumber = 1;

        // EITHER
        var bookToUpdate = library.Books.Single(b => b.ISDN == bookISDN);
        var chapterToUpdate = bookToUpdate.Chapters.Single(chapter => chapter.Number == chapterNumber);
        var updatedChapter = chapterToUpdate with
        {
            Pages = chapterToUpdate.Pages.Where(page => page.Number != pageNumber).ToArray()
        };
        var updatedBook = bookToUpdate with
        {
            Chapters = bookToUpdate.Chapters.Select(chapter => chapter.Number == chapterNumber ? updatedChapter : chapter).ToArray()
        };
        var updatedLibrary = library with
        {
            Books = library.Books.Select(book => book.ISDN == bookISDN ? updatedBook : book).ToArray()
        };

        // OR
        updatedLibrary = library with
        {
            Books = library.Books.Select(book => book.ISDN == bookISDN
                ? book with
                {
                    Chapters = book.Chapters.Select(chapter => chapter.Number == chapterNumber
                        ? chapter with
                        {
                            Pages = chapter.Pages.Where(page => page.Number != pageNumber).ToArray()
                        }
                        : chapter).ToArray()
                }
                : book).ToArray()
        };
        var newLibraryBook = updatedLibrary.Books.Single(b => b.ISDN == bookISDN);
        var newLibraryChapter = newLibraryBook.Chapters.Single(c => c.Number == chapterNumber);
        Assert.Empty(newLibraryChapter.Pages);
    }
}
