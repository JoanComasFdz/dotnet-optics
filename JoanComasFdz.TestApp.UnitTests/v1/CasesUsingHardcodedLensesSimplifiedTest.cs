using JoanComasFdz.Optics.Lenses.v1;
using JoanComasFdz.Optics.TestApp.Domain;
using static JoanComasFdz.Optics.TestApp.HowToUse.v1.Simplified.LibraryLensesSimplified;

namespace JoanComasFdz.TestApp.UnitTests.v1;

public class CasesUsingHardcodedLensesSimplifiedTest
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
    public void GetBookByISDN_BookIsCorret()
    {
        var bookISDN = "1234";

        var book = LibraryToBookLens(bookISDN).Get(library);

        Assert.NotNull(book);
        Assert.Equal(library.Books[0], book);
    }

    [Fact]
    public void SetBookByISDN_NewBookIsSet()
    {
        var bookISDN = "1234";
        var editedBook = library.Books[0] with { Title = "Second Title" };

        var newLibrary = LibraryToBookLens(bookISDN).Set(library, editedBook);

        Assert.NotNull(newLibrary);
        Assert.Equal(library.Name, newLibrary.Name);
        Assert.Equal(library.Books.Count, newLibrary.Books.Count);
        Assert.Equal(library.Books[0].ISDN, newLibrary.Books[0].ISDN);
        Assert.Equal(library.Books[0].Author, newLibrary.Books[0].Author);
        Assert.Equal("Second Title", newLibrary.Books[0].Title);
    }

    [Fact]
    public void UpdateBookByISDN_NewBookIsSet()
    {
        var bookISDN = "1234";

        var newLibrary = LibraryToBookLens(bookISDN).Update(library, book => book with { Title = "Second Title" });

        Assert.NotNull(newLibrary);
        Assert.Equal(library.Name, newLibrary.Name);
        Assert.Equal(library.Books.Count, newLibrary.Books.Count);
        Assert.Equal(library.Books[0].ISDN, newLibrary.Books[0].ISDN);
        Assert.Equal(library.Books[0].Author, newLibrary.Books[0].Author);
        Assert.Equal("Second Title", newLibrary.Books[0].Title);
    }

    [Fact]
    public void GetChapterByNumber_ChapterIsCorret()
    {
        var chapterNumber = 1;

        var chapter = BookToChapterLens(chapterNumber).Get(library.Books[0]);

        Assert.NotNull(chapter);
        Assert.Equal(library.Books[0].Chapters[0], chapter);
    }

    [Fact]
    public void SetChapterByNumber_NewChapterIsSet()
    {
        var chapterNumber = 1;
        var editedChapter = library.Books[0].Chapters[0] with { Title = "Second Title" };

        var newBook = BookToChapterLens(chapterNumber).Set(library.Books[0], editedChapter);

        Assert.NotNull(newBook);
        Assert.Equal(library.Books[0].Title, newBook.Title);
        Assert.Equal(library.Books[0].Chapters.Count, newBook.Chapters.Count);
        Assert.Equal(library.Books[0].Chapters[0].Number, newBook.Chapters[0].Number);
        Assert.Equal(library.Books[0].Chapters[0].Pages.Count, newBook.Chapters[0].Pages.Count);
        Assert.Equal("Second Title", newBook.Chapters[0].Title);
    }

    [Fact]
    public void UpdateChapterByNumber_NewChapterIsSet()
    {
        var chapterNumber = 1;

        var newBook = BookToChapterLens(chapterNumber).Update(library.Books[0], chapter => chapter with { Title = "Second Title" });

        Assert.NotNull(newBook);
        Assert.Equal(library.Books[0].Title, newBook.Title);
        Assert.Equal(library.Books[0].Chapters.Count, newBook.Chapters.Count);
        Assert.Equal(library.Books[0].Chapters[0].Number, newBook.Chapters[0].Number);
        Assert.Equal(library.Books[0].Chapters[0].Pages.Count, newBook.Chapters[0].Pages.Count);
        Assert.Equal("Second Title", newBook.Chapters[0].Title);
    }

    [Fact]
    public void GetPageByNumber_PageIsCorret()
    {
        var pageNumber = 1;

        var page = ChapterToPageLens(pageNumber).Get(library.Books[0].Chapters[0]);

        Assert.NotNull(page);
        Assert.Equal(library.Books[0].Chapters[0].Pages[0], page);
    }

    [Fact]
    public void SetPageByNumber_NewPageIsSet()
    {
        var pageNumber = 1;
        var editedPage = library.Books[0].Chapters[0].Pages[0] with { Content = "Second Content" };

        var newChapter = ChapterToPageLens(pageNumber).Set(library.Books[0].Chapters[0], editedPage);

        Assert.NotNull(newChapter);
        Assert.Equal(library.Books[0].Chapters[0].Title, newChapter.Title);
        Assert.Equal(library.Books[0].Chapters[0].Pages.Count, newChapter.Pages.Count);
        Assert.Equal(library.Books[0].Chapters[0].Number, newChapter.Number);
        Assert.Equal(library.Books[0].Chapters[0].Pages[0].Number, newChapter.Pages[0].Number);
        Assert.Equal("Second Content", newChapter.Pages[0].Content);
    }

    [Fact]
    public void UpdatePageByNumber_NewPageIsSet()
    {
        var pageNumber = 1;

        var newChapter = ChapterToPageLens(pageNumber).Update(library.Books[0].Chapters[0], page => page with { Content = "Second Content" });

        Assert.NotNull(newChapter);
        Assert.Equal(library.Books[0].Chapters[0].Title, newChapter.Title);
        Assert.Equal(library.Books[0].Chapters[0].Pages.Count, newChapter.Pages.Count);
        Assert.Equal(library.Books[0].Chapters[0].Number, newChapter.Number);
        Assert.Equal(library.Books[0].Chapters[0].Pages[0].Number, newChapter.Pages[0].Number);
        Assert.Equal("Second Content", newChapter.Pages[0].Content);
    }

    [Fact]
    public void HowToActuallyUseThem()
    {
        var bookISDN = "1234";
        var chapterNumber = 1;
        var pageNumber = 1;

        var newLibrary = LibraryToBookLens(bookISDN)
            .Compose(BookToChapterLens(chapterNumber))
            .Compose(ChapterToPageLens(pageNumber))
            .Update(library, page => page with { Content = "New Content" });

        Assert.Equal("New Content", newLibrary.Books[0].Chapters[0].Pages[0].Content);
    }
}

