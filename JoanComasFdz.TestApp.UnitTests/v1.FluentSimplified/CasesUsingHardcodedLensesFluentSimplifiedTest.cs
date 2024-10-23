using JoanComasFdz.Optics.Lenses.v1.Fluent;
using JoanComasFdz.Optics.TestApp.Domain;
using static JoanComasFdz.Optics.TestApp.UnitTests.v1.FluentSimplified.LibraryLensesFluentSimplified;

namespace JoanComasFdz.TestApp.UnitTests.v1.FluentSimplified;

public class CasesUsingHardcodedLensesFluentSimplifiedTest
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
    public void BookLensByISDN_WithNewBook_BookIsCorret()
    {
        var bookISDN = "1234";

        var newLibrary = library
            .BookLens(bookISDN)
            .With(book => book with { Title = "New book title" });

        Assert.NotNull(newLibrary);
        Assert.Equal(library.Name, newLibrary.Name);
        Assert.Equal(library.Books.Count, newLibrary.Books.Count);
        Assert.Equal(library.Books[0].ISDN, newLibrary.Books[0].ISDN);
        Assert.Equal(library.Books[0].Author, newLibrary.Books[0].Author);
        Assert.Equal(library.Books[0].Chapters.Count, newLibrary.Books[0].Chapters.Count);
        Assert.Equal("New book title", newLibrary.Books[0].Title);
    }

    [Fact]
    public void BookLensByISDN_ChapterLensWithNumber_WithNewChapter_ChapterIsCorret()
    {
        var bookISDN = "1234";
        var chapterNumber = 1;

        var newLibrary = library
            .BookLens(bookISDN)
            .ChapterLens(chapterNumber)
            .With(chapter => chapter with { Title = "New chapter title" });

        Assert.NotNull(newLibrary);
        Assert.Equal(library.Name, newLibrary.Name);
        Assert.Equal(library.Books.Count, newLibrary.Books.Count);
        Assert.Equal(library.Books[0].Chapters.Count, newLibrary.Books[0].Chapters.Count);
        Assert.Equal(library.Books[0].Chapters[0].Number, newLibrary.Books[0].Chapters[0].Number);
        Assert.Equal("New chapter title", newLibrary.Books[0].Chapters[0].Title);
    }

    [Fact]
    public void BookLensByISDN_ChapterLensWithNumber_PageLensByNumber_WithNewPage_PageIsCorret()
    {
        var bookISDN = "1234";
        var chapterNumber = 1;
        var pageNumber = 1;

        var newLibrary = library
            .BookLens(bookISDN)
            .ChapterLens(chapterNumber)
            .PageLens(pageNumber)
            .With(page => page with { Content = "New page content" });

        Assert.NotNull(newLibrary);
        Assert.Equal(library.Name, newLibrary.Name);
        Assert.Equal(library.Books.Count, newLibrary.Books.Count);
        Assert.Equal(library.Books[0].Chapters.Count, newLibrary.Books[0].Chapters.Count);
        Assert.Equal(library.Books[0].Chapters[0].Pages.Count, newLibrary.Books[0].Chapters[0].Pages.Count);
        Assert.Equal(library.Books[0].Chapters[0].Pages[0].Number, newLibrary.Books[0].Chapters[0].Pages[0].Number);
        Assert.Equal("New page content", newLibrary.Books[0].Chapters[0].Pages[0].Content);
    }
}