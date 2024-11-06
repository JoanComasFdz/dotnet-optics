using static JoanComasFdz.Optics.Usage.UnitTests.v2.LibraryLenses;
using JoanComasFdz.Optics.Lenses.v2;

namespace JoanComasFdz.Optics.Usage.UnitTests.v2;

public class CasesUsingHardcodedLensTest
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
        Assert.Equal("5678", newLibrary.Books[1].ISDN);
    }

    [Fact]
    public void AddChapterToBook_BookHasNewChapter()
    {
        var secondChapter = new Chapter(
            Number: 2,
            Title: "First Algorithms",
            Pages: [
                new Page(10, "Page 10 Content")
            ]
        );

        var newLibrary = LibraryToBookLens("1234")
            .Compose(BookToChaptersLens())
            .Update(library, chapters => [.. chapters, secondChapter]);

        var updatedBook = newLibrary.Books.Single(b => b.ISDN == "1234");
        Assert.Equal(2, updatedBook.Chapters.Count);
        Assert.Equal("First Algorithms", updatedBook.Chapters[1].Title);
    }

    [Fact]
    public void AddPageToChapterOfBook_ChapterHasNewPage()
    {
        var newLibrary = LibraryToBookLens("1234")
            .Compose(BookToChapterLens(1))
            .Compose(ChapterToPagesLens())
            .Update(library, pages => [.. pages, new Page(2, "Page 2 Content")]);

        var updatedBook = newLibrary.Books.Single(b => b.ISDN == "1234");
        var updatedChapter = updatedBook.Chapters.Single(c => c.Number == 1);
        Assert.Equal(2, updatedChapter.Pages.Count);
        Assert.Equal("Page 2 Content", updatedChapter.Pages[1].Content);
    }

    [Fact]
    public void UpdateBookTitle_BookTitleIsUpdated()
    {
        var newLibrary = LibraryToBookLens("1234").Update(library, book => book with { Title = "Program nicely" });

        var updatedBook = newLibrary.Books.Single(b => b.ISDN == "1234");
        Assert.Equal("Program nicely", updatedBook.Title);
    }

    [Fact]
    public void UpdateChapterTitleOfBook_ChapterTitleIsUpdated()
    {
        var newLibrary = LibraryToBookLens("1234")
            .Compose(BookToChapterLens(1))
            .Update(library, chapter => chapter with { Title = "Advanced Algorithms" });

        var updatedBook = newLibrary.Books.Single(b => b.ISDN == "1234");
        var updatedChapter = updatedBook.Chapters.Single(c => c.Number == 1);
        Assert.Equal("Advanced Algorithms", updatedChapter.Title);
    }

    [Fact]
    public void UpdatePageContentOfChapterOfBook_PageContentIsUpdated()
    {
        var newLibrary = LibraryToBookLens("1234")
            .Compose(BookToChapterLens(1))
            .Compose(ChapterToPageLens(1))
            .Update(library, page => page with { Content = "Updated Content" });

        var updatedBook = newLibrary.Books.Single(b => b.ISDN == "1234");
        var updatedChapter = updatedBook.Chapters.Single(c => c.Number == 1);
        var updatedPage = updatedChapter.Pages.Single(p => p.Number == 1);
        Assert.Equal("Updated Content", updatedPage.Content);
    }

    [Fact]
    public void RemoveBookFromLibrary_BookIsRemoved()
    {
        var newLibrary = LibraryToBooksLens().Update(library, books => books.Where(book => book.ISDN != "1234").ToArray());

        Assert.Empty(newLibrary.Books);
    }

    [Fact]
    public void RemoveChapterFromBook_ChapterIsRemoved()
    {
        var newLibrary = LibraryToBookLens("1234")
            .Compose(BookToChaptersLens())
            .Update(library, chapters => chapters.Where(chapter => chapter.Number != 1).ToArray());

        var updatedBook = newLibrary.Books.Single(b => b.ISDN == "1234");
        Assert.Empty(updatedBook.Chapters);
    }

    [Fact]
    public void RemovePageFromChapterOfBook_PageIsRemoved()
    {
        var newLibrary = LibraryToBookLens("1234")
            .Compose(BookToChapterLens(1))
            .Compose(ChapterToPagesLens())
            .Update(library, pages => pages.Where(page => page.Number != 1).ToArray());

        var updatedBook = newLibrary.Books.Single(b => b.ISDN == "1234");
        var updatedChapter = updatedBook.Chapters.Single(c => c.Number == 1);
        Assert.Empty(updatedChapter.Pages);
    }
}