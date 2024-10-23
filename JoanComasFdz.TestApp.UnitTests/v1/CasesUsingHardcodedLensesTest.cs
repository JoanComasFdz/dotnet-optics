using JoanComasFdz.Optics.Lenses.v1;
using static JoanComasFdz.TestApp.UnitTests.v1.LibraryLenses;

namespace JoanComasFdz.TestApp.UnitTests.v1;

public class CasesUsingHardcodedLensesTest
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

        var newLibrary = LibraryToBooksLens().Update(library, books => [.. books, secondBook]);

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

        var newLibrary = LibraryToBookLens(book => book.ISDN == bookISDN)
            .Compose(BookToChaptersLens())
            .Update(library, chapters => [.. chapters, secondChapter]);

        var updatedBook = newLibrary.Books.Single(b => b.ISDN == bookISDN);
        Assert.Equal(2, updatedBook.Chapters.Count);
        Assert.Equal("First Algorithms", updatedBook.Chapters.Last().Title);
    }

    [Fact]
    public void AddPageToChapterOfBook_ChapterHasNewPage()
    {
        var bookISDN = "1234";
        var chapterNumber = 1;

        var newLibrary = LibraryToBookLens(book => book.ISDN == bookISDN)
            .Compose(BookToChapterLens(chapter => chapter.Number == chapterNumber))
            .Compose(ChapterToPagesLens())
            .Update(library, pages => [.. pages, new Page(2, "Page 2 Content")]);

        var updatedBook = newLibrary.Books.Single(b => b.ISDN == bookISDN);
        var updatedChapter = updatedBook.Chapters.Single(c => c.Number == chapterNumber);
        Assert.Equal(2, updatedChapter.Pages.Count);
        Assert.Equal("Page 2 Content", updatedChapter.Pages.Last().Content);
    }

    [Fact]
    public void UpdateBookTitle_BookTitleIsUpdated()
    {
        var bookISDN = "1234";

        var newLibrary = LibraryToBookLens(book => book.ISDN == bookISDN)
            .Update(library, book => book with { Title = "Program nicely" });

        var updatedBook = newLibrary.Books.Single(b => b.ISDN == bookISDN);
        Assert.Equal("Program nicely", updatedBook.Title);
    }

    [Fact]
    public void UpdateChapterTitleOfBook_ChapterTitleIsUpdated()
    {
        var bookISDN = "1234";
        var chapterNumber = 1;

        var newLibrary = LibraryToBookLens(book => book.ISDN == bookISDN)
            .Compose(BookToChapterLens(chapter => chapter.Number == chapterNumber))
            .Update(library, chapter => chapter with { Title = "Advanced Algorithms" });

        var updatedBook = newLibrary.Books.Single(b => b.ISDN == bookISDN);
        var updatedChapter = updatedBook.Chapters.Single(c => c.Number == chapterNumber);
        Assert.Equal("Advanced Algorithms", updatedChapter.Title);
    }

    [Fact]
    public void UpdatePageContentOfChapterOfBook_PageContentIsUpdated()
    {
        var bookISDN = "1234";
        var chapterNumber = 1;
        var pageNumber = 1;

        var newLibrary = LibraryToBookLens(book => book.ISDN == bookISDN)
            .Compose(BookToChapterLens(chapter => chapter.Number == pageNumber))
            .Compose(ChapterToPageLens(page => page.Number == chapterNumber))
            .Update(library, page => page with { Content = "Updated Content" });

        var updatedBook = newLibrary.Books.Single(b => b.ISDN == bookISDN);
        var updatedChapter = updatedBook.Chapters.Single(c => c.Number == chapterNumber);
        var updatedPage = updatedChapter.Pages.Single(p => p.Number == pageNumber);
        Assert.Equal("Updated Content", updatedPage.Content);
    }

    [Fact]
    public void RemoveBookFromLibrary_BookIsRemoved()
    {
        var bookISDN = "1234";

        var newLibrary = LibraryToBooksLens().Update(library, books => books.Where(book => book.ISDN != bookISDN).ToArray());

        Assert.Empty(newLibrary.Books);
    }

    [Fact]
    public void RemoveChapterFromBook_ChapterIsRemoved()
    {
        var bookISDN = "1234";
        var chapterNumber = 1;

        var newLibrary = LibraryToBookLens(book => book.ISDN == bookISDN)
            .Compose(BookToChaptersLens())
            .Update(library, chapters => chapters.Where(chapter => chapter.Number != chapterNumber).ToArray());

        var updatedBook = newLibrary.Books.Single(b => b.ISDN == bookISDN);
        Assert.Empty(updatedBook.Chapters);
    }

    [Fact]
    public void RemovePageFromChapterOfBook_PageIsRemoved()
    {
        var bookISDN = "1234";
        var chapterNumber = 1;
        var pageNumber = 1;
        var newLibrary = LibraryToBookLens(book => book.ISDN == bookISDN)
            .Compose(BookToChapterLens(chapter => chapter.Number == chapterNumber))
            .Compose(ChapterToPagesLens())
            .Update(library, pages => pages.Where(page => page.Number != pageNumber).ToArray());

        var updatedBook = newLibrary.Books.Single(b => b.ISDN == bookISDN);
        var updatedChapter = updatedBook.Chapters.Single(c => c.Number == pageNumber);
        Assert.Empty(updatedChapter.Pages);
    }
}