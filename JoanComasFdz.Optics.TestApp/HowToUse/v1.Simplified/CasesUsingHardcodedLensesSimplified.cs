using JoanComasFdz.Optics.Lenses;
using JoanComasFdz.Optics.Lenses.v1;
using JoanComasFdz.Optics.TestApp.Domain;
using static JoanComasFdz.Optics.TestApp.HowToUse.v1.LibraryLenses;
using static JoanComasFdz.Optics.TestApp.HowToUse.v1.Simplified.LibraryLensesSimplified;

namespace JoanComasFdz.Optics.TestApp.HowToUse.v1.Simplified;

public static class CasesUsingHardcodedLensesSimplified
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

        return LibraryToBooksLens().Update(library, books => [.. books, secondBook]);
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

        return LibraryToBookLens(bookISDN)
            .Compose(BookToChaptersLens())
            .Update(library, chapters => [.. chapters, secondChapter]);
    }

    public static Library AddPageToChapterOfBook(Library library, string bookISDN, int chapterNumber)
    {
        return LibraryToBookLens(bookISDN)
            .Compose(BookToChapterLens(chapterNumber))
            .Compose(ChapterToPagesLens())
            .Update(library, pages => [.. pages, new Page(2, "Page 2 Content")]);
    }

    public static Library UpdateBookTitle(Library library, string bookISDN, string newTitle)
    {
        return LibraryToBookLens(bookISDN)
            .Update(library, book => book with { Title = newTitle });
    }

    public static Library UpdateChapterTitleOfBook(Library library, string bookISDN, int chapterNumber, string newTitle)
    {
        return LibraryToBookLens(bookISDN)
            .Compose(BookToChapterLens(chapterNumber))
            .Update(library, chapter => chapter with { Title = newTitle });
    }

    public static Library UpdatePageContentOfChapterOfBook(Library library, string bookISDN, int chapterNumber, int pageNumber, string newContent)
    {
        return LibraryToBookLens(bookISDN)
            .Compose(BookToChapterLens(chapterNumber))
            .Compose(ChapterToPageLens(pageNumber))
            .Update(library, page => page with { Content = newContent });
    }

    public static Library RemoveBookFromLibrary(Library library, string bookISDN)
    {
        return LibraryToBooksLens().Update(library, books => books.Where(book => book.ISDN != bookISDN).ToArray());
    }

    public static Library RemoveChapterFromBook(Library library, string bookISDN, int chapterNumber)
    {
        return LibraryToBookLens(bookISDN)
            .Compose(BookToChaptersLens())
            .Update(library, chapters => chapters.Where(chapter => chapter.Number != chapterNumber).ToArray());
    }

    public static Library RemovePageFromChapterOfBook(Library library, string bookISDN, int chapterNumber, int pageNumber)
    {
        return LibraryToBookLens(bookISDN)
            .Compose(BookToChapterLens(chapterNumber))
            .Compose(ChapterToPagesLens())
            .Update(library, pages => pages.Where(page => page.Number != pageNumber).ToArray());
    }
}
