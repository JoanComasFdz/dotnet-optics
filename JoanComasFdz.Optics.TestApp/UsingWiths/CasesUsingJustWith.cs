using JoanComasFdz.Optics.TestApp.Domain;

namespace JoanComasFdz.Optics.TestApp.UsingWiths;

internal static class CasesUsingJustWith
{
    public static Library AddBookToLibrary(Library library)
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

        var updatedLibrary = library with
        {
            Books = [.. library.Books, secondBook]
        };

        return updatedLibrary;
    }

    public static Library AddChapterToBook(Library library, string bookISDN)
    {
        var secondChapter = new Chapter(
            Number: 2,
            Title: "First Algorithms",
            Pages: [
                new Page(10, "Page 10 Content")
            ]
        );

        var bookToUpdate = library.Books.Single(b => b.ISDN == bookISDN);
        var updatedBookWithNewChapter = bookToUpdate with
        {
            Chapters = [.. bookToUpdate.Chapters, secondChapter]
        };
        var updatedLibrary = library with
        {
            Books = library.Books.Select(book => book.ISDN == bookISDN ? updatedBookWithNewChapter : book).ToArray()
        };

        return updatedLibrary;

        // Single statement
        return library with
        {
            Books = library.Books.Select(book => book.ISDN == bookISDN
                ? book with
                {
                    Chapters = [.. book.Chapters, secondChapter]
                }
                : book).ToArray()
        };
    }

    public static Library AddPageToChapterOfBook(Library library, string bookISDN, int chapterNumber)
    {
        var secondPage = new Page(2, "Page 2 Content");

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
        var updatedLibrary = library with
        {
            Books = library.Books.Select(book => book.ISDN == bookISDN ? updatedBookWithUpdatedChapter : book).ToArray()
        };

        return updatedLibrary;

        // Single statement
        return library with
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
    }

    public static Library UpdateBookTitle(Library library, string bookISDN, string newTitle)
    {
        var newBookTitle = "Program nicely";

        var bookToUpdate = library.Books.Single(book => book.ISDN == bookISDN);
        var updatedBook = bookToUpdate with
        {
            Title = newBookTitle,
        };
        var updatedLibrary = library with
        {
            Books = library.Books.Select(book => book.ISDN == bookISDN ? updatedBook : book).ToArray()
        };
        return updatedLibrary;

        // Single statement
        return library with
        {
            Books = library.Books.Select(book => book.ISDN == bookISDN
                                            ? book with
                                            {
                                                Title = newBookTitle
                                            }
                                            : book)
                                        .ToArray()
        };
    }

    public static Library UpdateChapterTitleOfBook(Library library, string bookISDN, int chapterNumber, string newTitle)
    {
        var bookToUpdate = library.Books.Single(book => book.ISDN == bookISDN);
        var chapterToUpdate = bookToUpdate.Chapters.Single(chapter => chapter.Number == chapterNumber);
        var updatedChapter = chapterToUpdate with
        {
            Title = newTitle
        };
        var bookWithUpdatedChapter = bookToUpdate with
        {
            Chapters = bookToUpdate.Chapters.Select(chapter => chapter.Number == chapterNumber ? updatedChapter : chapter).ToArray()
        };
        var updatedLibrary = library with
        {
            Books = library.Books.Select(book => book.ISDN == bookISDN ? bookWithUpdatedChapter : book).ToArray()
        };

        return updatedLibrary;

        // Single statement
        return library with
        {
            Books = library.Books.Select(book => book.ISDN == bookISDN
                ? book with
                {
                    Chapters = book.Chapters.Select(chapter => chapter.Number == chapterNumber
                        ? chapter with
                        {
                            Title = newTitle
                        }
                        : chapter).ToArray()
                }
                : book).ToArray()
        };
    }

    public static Library UpdatePageContentOfChapterOfBook(Library library, string bookISDN, int chapterNumber, int pageNumber, string newContent)
    {
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

        return updatedLibrary;

        // Single statement
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
    }

    public static Library RemoveBookFromLibrary(Library library, string bookISDN)
    {
        return library with
        {
            Books = library.Books.Where(book => book.ISDN != bookISDN).ToArray()
        };
    }

    public static Library RemoveChapterFromBook(Library library, string bookISDN, int chapterNumber)
    {
        var bookToUpdate = library.Books.Single(b => b.ISDN == bookISDN);
        var updatedBook = bookToUpdate with
        {
            Chapters = bookToUpdate.Chapters.Where(chapter => chapter.Number != chapterNumber).ToArray()
        };
        var updatedLibrary = library with
        {
            Books = library.Books.Select(book => book.ISDN == bookISDN ? updatedBook : book).ToArray()
        };
        return updatedLibrary;

        // Single statement
        return library with
        {
            Books = library.Books.Select(book => book.ISDN == bookISDN
                ? book with
                {
                    Chapters = book.Chapters.Where(chapter => chapter.Number != chapterNumber).ToArray()
                }
                : book).ToArray()
        };
    }

    public static Library RemovePageFromChapterOfBook(Library library, string bookISDN, int chapterNumber, int pageNumber)
    {
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
        return updatedLibrary;

        // Single statement
        return library with
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
    }
}
