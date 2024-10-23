using JoanComasFdz.Optics.TestApp.Domain;
using JoanComasFdz.Optics.Lenses.v1;
using JoanComasFdz.Optics.TestApp.UnitTests.v1.Simplified;
using JoanComasFdz.Optics.Lenses.v1.Fluent;

namespace JoanComasFdz.Optics.TestApp.UnitTests.v1.FluentSimplified;

public static class LibraryLensesFluentSimplified
{
    public static LensWrapper<Library, Book> BookLens(this Library library, string bookISDN)
    {
        var lens = LibraryLensesSimplified.LibraryToBookLens(bookISDN);
        return new LensWrapper<Library, Book>(library, lens);
    }

    public static LensWrapper<Library, Chapter> ChapterLens(this LensWrapper<Library, Book> wrapper, int chapterNumber)
    {
        // Create the lens for accessing the Chapter within the Book
        var chapterLens = LibraryLensesSimplified.BookToChapterLens(chapterNumber);

        // Compose the current Lens (from Library to Book) with the new Lens (from Book to Chapter)
        var composedLens = wrapper.Lens.Compose(chapterLens);

        // Return a new LensWrapper that wraps the Library and the composed Lens to focus on the Chapter
        return new LensWrapper<Library, Chapter>(wrapper.Whole, composedLens);
    }

    public static LensWrapper<Library, Page> PageLens(this LensWrapper<Library, Chapter> wrapper, int pageNumber)
    {
        // Create the lens for accessing the Page within the Chapter
        var pageLens = LibraryLensesSimplified.ChapterToPageLens(pageNumber);

        // Compose the current Lens (from Library to Chapter) with the new Lens (from Chapter to Page)
        var composedLens = wrapper.Lens.Compose(pageLens);

        // Return a new LensWrapper that wraps the Library and the composed Lens to focus on the Page
        return new LensWrapper<Library, Page>(wrapper.Whole, composedLens);
    }
}