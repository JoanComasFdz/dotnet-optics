using JoanComasFdz.Optics.Lenses.v1;

namespace JoanComasFdz.TestApp.UnitTests.v1.Simplified;

public static class LibraryLensesSimplified
{
    public static Lens<Library, Book> LibraryToBookLens(string bookISDN)
    {
        return new Lens<Library, Book>(
            library => library.Books.Single(book => book.ISDN == bookISDN),
            (library, updatedBook) =>
            {
                var updatedBooks = library.Books.Select(book => book.ISDN == bookISDN ? updatedBook : book).ToList();
                return library with { Books = updatedBooks.AsReadOnly() };
            }
        );
    }

    public static Lens<Book, Chapter> BookToChapterLens(int chapterNumber)
    {
        return new Lens<Book, Chapter>(
            book => book.Chapters.Single(chapter => chapter.Number == chapterNumber),
            (book, updatedChapter) =>
            {
                var updatedChapters = book.Chapters.Select(chapter => chapter.Number == chapterNumber ? updatedChapter : chapter).ToList();
                return book with { Chapters = updatedChapters.AsReadOnly() };
            }
        );
    }

    public static Lens<Chapter, Page> ChapterToPageLens(int pageNumber)
    {
        return new Lens<Chapter, Page>(
            chapter => chapter.Pages.Single(page => page.Number == pageNumber),
            (chapter, updatedPage) =>
            {
                var updatedPages = chapter.Pages.Select(page => page.Number == pageNumber ? updatedPage : page).ToList();
                return chapter with { Pages = updatedPages.AsReadOnly() };
            }
        );
    }
}