using JoanComasFdz.Optics.Lenses;
using JoanComasFdz.Optics.TestApp.Domain;

namespace JoanComasFdz.Optics.TestApp.UsingHardcodedLensesSimplified;

public static class LibraryLensesSimplified
{
    public static OldLens<Library, Book> LibraryToBookLens(string bookISDN)
    {
        return new OldLens<Library, Book>(
            library => library.Books.Single(book => book.ISDN == bookISDN),
            (library, updatedBook) =>
            {
                var updatedBooks = library.Books.Select(book => book.ISDN == bookISDN ? updatedBook : book).ToList();
                return library with { Books = updatedBooks.AsReadOnly() };
            }
        );
    }

    public static OldLens<Book, Chapter> BookToChapterLens(int chapterNumber)
    {
        return new OldLens<Book, Chapter>(
            book => book.Chapters.Single(chapter => chapter.Number == chapterNumber),
            (book, updatedChapter) =>
            {
                var updatedChapters = book.Chapters.Select(chapter => chapter.Number == chapterNumber ? updatedChapter : chapter).ToList();
                return book with { Chapters = updatedChapters.AsReadOnly() };
            }
        );
    }

    public static OldLens<Chapter, Page> ChapterToPageLens(int pageNumber)
    {
        return new OldLens<Chapter, Page>(
            chapter => chapter.Pages.Single(page => page.Number == pageNumber),
            (chapter, updatedPage) =>
            {
                var updatedPages = chapter.Pages.Select(page => page.Number == pageNumber ? updatedPage : page).ToList();
                return chapter with { Pages = updatedPages.AsReadOnly() };
            }
        );
    }
}