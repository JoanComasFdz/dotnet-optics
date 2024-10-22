using JoanComasFdz.Optics.TestApp.Domain;
using JoanComasFdz.Optics.TestApp.HowToUse.v1.Fluent;

namespace JoanComasFdz.TestApp.UnitTests;

public class CasesUsingEnhancedApiTest
{
    private Library library = new Library(
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
        var newLibrary = CasesUsingHardcodedLensesFluent.AddBookToLibrary(library);

        Assert.Equal(2, newLibrary.Books.Count);
        Assert.Equal("5678", newLibrary.Books.Last().ISDN);
    }

    [Fact]
    public void AddChapterToBook_BookHasNewChapter()
    {
        var newLibrary = CasesUsingHardcodedLensesFluent.AddChapterToBook(library, "1234");

        var updatedBook = newLibrary.Books.Single(b => b.ISDN == "1234");
        Assert.Equal(2, updatedBook.Chapters.Count);
        Assert.Equal("First Algorithms", updatedBook.Chapters.Last().Title);
    }

    [Fact]
    public void AddPageToChapterOfBook_ChapterHasNewPage()
    {
        var newLibrary = CasesUsingHardcodedLensesFluent.AddPageToChapterOfBook(library, "1234", 1);

        var updatedBook = newLibrary.Books.Single(b => b.ISDN == "1234");
        var updatedChapter = updatedBook.Chapters.Single(c => c.Number == 1);
        Assert.Equal(2, updatedChapter.Pages.Count);
        Assert.Equal("Page 2 Content", updatedChapter.Pages.Last().Content);
    }

    [Fact]
    public void UpdateBookTitle_BookTitleIsUpdated()
    {
        var newLibrary = CasesUsingHardcodedLensesFluent.UpdateBookTitle(library, "1234", "Program nicely");

        var updatedBook = newLibrary.Books.Single(b => b.ISDN == "1234");
        Assert.Equal("Program nicely", updatedBook.Title);
    }

    [Fact]
    public void UpdateChapterTitleOfBook_ChapterTitleIsUpdated()
    {
        var newLibrary = CasesUsingHardcodedLensesFluent.UpdateChapterTitleOfBook(library, "1234", 1, "Advanced Algorithms");

        var updatedBook = newLibrary.Books.Single(b => b.ISDN == "1234");
        var updatedChapter = updatedBook.Chapters.Single(c => c.Number == 1);
        Assert.Equal("Advanced Algorithms", updatedChapter.Title);
    }

    [Fact]
    public void UpdatePageContentOfChapterOfBook_PageContentIsUpdated()
    {
        var newLibrary = CasesUsingHardcodedLensesFluent.UpdatePageContentOfChapterOfBook(library, "1234", 1, 1, "Updated Content");

        var updatedBook = newLibrary.Books.Single(b => b.ISDN == "1234");
        var updatedChapter = updatedBook.Chapters.Single(c => c.Number == 1);
        var updatedPage = updatedChapter.Pages.Single(p => p.Number == 1);
        Assert.Equal("Updated Content", updatedPage.Content);
    }

    [Fact]
    public void RemoveBookFromLibrary_BookIsRemoved()
    {
        var newLibrary = CasesUsingHardcodedLensesFluent.RemoveBookFromLibrary(library, "1234");

        Assert.Empty(newLibrary.Books);
    }

    [Fact]
    public void RemoveChapterFromBook_ChapterIsRemoved()
    {
        var newLibrary = CasesUsingHardcodedLensesFluent.RemoveChapterFromBook(library, "1234", 1);

        var updatedBook = newLibrary.Books.Single(b => b.ISDN == "1234");
        Assert.Empty(updatedBook.Chapters);
    }

    [Fact]
    public void RemovePageFromChapterOfBook_PageIsRemoved()
    {
        var newLibrary = CasesUsingHardcodedLensesFluent.RemovePageFromChapterOfBook(library, "1234", 1, 1);

        var updatedBook = newLibrary.Books.Single(b => b.ISDN == "1234");
        var updatedChapter = updatedBook.Chapters.Single(c => c.Number == 1);
        Assert.Empty(updatedChapter.Pages);
    }
}