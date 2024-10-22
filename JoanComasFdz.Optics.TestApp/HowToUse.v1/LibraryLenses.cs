using JoanComasFdz.Optics.Lenses;
using JoanComasFdz.Optics.TestApp.Domain;

namespace JoanComasFdz.Optics.TestApp.HowToUse.v1;

public static class LibraryLenses
{
    public static Lens<Library, IReadOnlyList<Book>> LibraryToBooksLens() => new(
        library => library.Books,
        (library, newBooks) => library with { Books = newBooks }
    );

    public static Lens<Library, Book> LibraryToBookLens(Func<Book, bool> predicate)
    {
        return new Lens<Library, Book>(
            library => library.Books.Single(predicate),
            (library, updatedBook) => library with { Books = library.Books.Select(book => predicate(book) ? updatedBook : book).ToArray() }
        );
    }

    public static Lens<Book, IReadOnlyList<Chapter>> BookToChaptersLens() => new(
        book => book.Chapters,
        (book, newChapters) => book with { Chapters = newChapters }
    );

    public static Lens<Book, Chapter> BookToChapterLens(Func<Chapter, bool> predicate)
    {
        return new Lens<Book, Chapter>(
            book => book.Chapters.Single(predicate),
            (book, updatedChapter) =>
            {
                var updatedChapters = book.Chapters.Select(chapter => predicate(chapter) ? updatedChapter : chapter).ToList();
                return book with { Chapters = updatedChapters.AsReadOnly() };
            }
        );
    }

    public static Lens<Chapter, IReadOnlyList<Page>> ChapterToPagesLens() => new(
        chapter => chapter.Pages,
        (chapter, newPages) => chapter with { Pages = newPages }
    );

    public static Lens<Chapter, Page> ChapterToPageLens(Func<Page, bool> predicate)
    {
        return new Lens<Chapter, Page>(
            chapter => chapter.Pages.Single(predicate),
            (chapter, updatedPage) =>
            {
                var updatedPages = chapter.Pages.Select(page => predicate(page) ? updatedPage : page).ToList();
                return chapter with { Pages = updatedPages.AsReadOnly() };
            }
        );
    }
}