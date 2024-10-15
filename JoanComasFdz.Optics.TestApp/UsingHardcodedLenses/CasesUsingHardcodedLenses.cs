using JoanComasFdz.Optics.Lenses;
using Optics.TestApp.Domain;
using static JoanComasFdz.Optics.TestApp.UsingHardcodedLenses.LibraryLenses;

namespace JoanComasFdz.Optics.TestApp.UsingHardcodedLenses;

public static class CasesUsingHardcodedLenses
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

        return LibraryToBooksLens().Mutate(library, books => [.. books, secondBook]);
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

        return LibraryToBookLens(book => book.ISDN == bookISDN)
            .Compose(BookToChaptersLens())
            .Mutate(library, chapters => [.. chapters, secondChapter]);
    }

    public static Library AddPageToChapterOfBook(Library library, string bookISDN, int chapterNumber)
    {
        return LibraryToBookLens(book => book.ISDN == bookISDN)
            .Compose(BookToChapterLens(chapter => chapter.Number == chapterNumber))
            .Compose(ChapterToPagesLens())
            .Mutate(library, pages => [.. pages, new Page(2, "Page 2 Content")]);
    }

    public static Library UpdateBookTitle(Library library, string bookISDN, string newTitle)
    {
        return LibraryToBookLens(book => book.ISDN == bookISDN)
            .Mutate(library, book => book with { Title = newTitle });
    }

    public static Library UpdateChapterTitleOfBook(Library library, string bookISDN, int chapterNumber, string newTitle)
    {
        return LibraryToBookLens(book => book.ISDN == bookISDN)
            .Compose(BookToChapterLens(chapter => chapter.Number == chapterNumber))
            .Mutate(library, chapter => chapter with { Title = newTitle });
    }

    public static Library UpdatePageContentOfChapterOfBook(Library library, string bookISDN, int chapterNumber, int pageNumber, string newContent)
    {
        return LibraryToBookLens(book => book.ISDN == bookISDN)
            .Compose(BookToChapterLens(chapter => chapter.Number == pageNumber))
            .Compose(ChapterToPageLens(page => page.Number == chapterNumber))
            .Mutate(library, page => page with { Content = newContent });
    }

    public static Library RemoveBookFromLibrary(Library library, string bookISDN)
    {
        return LibraryToBooksLens().Mutate(library, books => books.Where(book => book.ISDN != bookISDN).ToArray());
    }

    public static Library RemoveChapterFromBook(Library library, string bookISDN, int chapterNumber)
    {
        return LibraryToBookLens(book => book.ISDN == bookISDN)
            .Compose(BookToChaptersLens())
            .Mutate(library, chapters => chapters.Where(chapter => chapter.Number != chapterNumber).ToArray());
    }

    public static Library RemovePageFromChapterOfBook(Library library, string bookISDN, int chapterNumber, int pageNumber)
    {
        return LibraryToBookLens(book => book.ISDN == bookISDN)
            .Compose(BookToChapterLens(chapter => chapter.Number == chapterNumber))
            .Compose(ChapterToPagesLens())
            .Mutate(library, pages => pages.Where(page => page.Number != pageNumber).ToArray());
    }
}
