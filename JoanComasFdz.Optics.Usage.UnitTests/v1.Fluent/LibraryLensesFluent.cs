using JoanComasFdz.Optics.Lenses.v1;
using JoanComasFdz.Optics.Lenses.v1.Fluent;

namespace JoanComasFdz.Optics.Usage.UnitTests.v1.Fluent;

public static class LibraryLensesFluent
{
    public static LensWrapper<Library, IReadOnlyList<Book>> BooksLens(this Library library)
    {
        var booksLens = LibraryLenses.LibraryToBooksLens();

        return new LensWrapper<Library, IReadOnlyList<Book>>(library, booksLens);
    }

    public static LensWrapper<Library, Book> BookLens(this Library library, Func<Book, bool> predicate)
    {
        var lens = LibraryLenses.LibraryToBookLens(predicate);
        return new LensWrapper<Library, Book>(library, lens);
    }

    public static LensWrapper<Library, IReadOnlyList<Chapter>> ChaptersLens(this LensWrapper<Library, Book> wrapper)
    {
        // Create the lens for accessing the Chapter collection within the Book
        var chaptersLens = LibraryLenses.BookToChaptersLens();

        // Compose the current Lens (from Library to Book) with the new Lens (from Book to Chapters)
        var composedLens = wrapper.Lens.Compose(chaptersLens);

        // Return a new LensWrapper that wraps the Library and the composed Lens to focus on the Chapters
        return new LensWrapper<Library, IReadOnlyList<Chapter>>(wrapper.Whole, composedLens);
    }

    public static LensWrapper<Library, Chapter> ChapterLens(this LensWrapper<Library, Book> wrapper, Func<Chapter, bool> predicate)
    {
        // Create the lens for accessing the Chapter within the Book
        var chapterLens = LibraryLenses.BookToChapterLens(predicate);

        // Compose the current Lens (from Library to Book) with the new Lens (from Book to Chapter)
        var composedLens = wrapper.Lens.Compose(chapterLens);

        // Return a new LensWrapper that wraps the Library and the composed Lens to focus on the Chapter
        return new LensWrapper<Library, Chapter>(wrapper.Whole, composedLens);
    }

    public static LensWrapper<Library, IReadOnlyList<Page>> PagesLens(this LensWrapper<Library, Chapter> wrapper)
    {
        var pagesLens = LibraryLenses.ChapterToPagesLens();

        // Compose the existing Lens from Library -> Chapter with the new Lens Chapter -> Pages
        var composedLens = wrapper.Lens.Compose(pagesLens);

        // Return a new LensWrapper that wraps the Library and the composed Lens to focus on the Pages
        return new LensWrapper<Library, IReadOnlyList<Page>>(wrapper.Whole, composedLens);
    }

    public static LensWrapper<Library, Page> PageLens(this LensWrapper<Library, Chapter> wrapper, Func<Page, bool> predicate)
    {
        // Create the lens for accessing the Page within the Chapter
        var pageLens = LibraryLenses.ChapterToPageLens(predicate);

        // Compose the current Lens (from Library to Chapter) with the new Lens (from Chapter to Page)
        var composedLens = wrapper.Lens.Compose(pageLens);

        // Return a new LensWrapper that wraps the Library and the composed Lens to focus on the Page
        return new LensWrapper<Library, Page>(wrapper.Whole, composedLens);
    }
}
