using JoanComasFdz.Optics.Lenses.v2;

namespace JoanComasFdz.Optics.Usage.UnitTests.v2;

public static class LibraryLenses
{
    public static Lens<Library, Address> LibraryToAddress()
        => Lens<Library, Address>.Create(library => library.Address);

    public static Lens<Library, IReadOnlyList<Book>> LibraryToBooksLens()
        => Lens<Library, IReadOnlyList<Book>>.Create(library => library.Books);

    public static Lens<Library, Book> LibraryToBookLens(string bookISDN)
        => Lens<Library, Book>.Create<IReadOnlyCollection<Book>>(library => library.Books, book => book.ISDN == bookISDN);

    public static Lens<Book, IReadOnlyList<Chapter>> BookToChaptersLens()
        => Lens<Book, IReadOnlyList<Chapter>>.Create(book => book.Chapters);

    public static Lens<Book, Chapter> BookToChapterLens(int chapterNumber)
        => Lens<Book, Chapter>.Create<IReadOnlyList<Chapter>>(book => book.Chapters, chapter => chapter.Number == chapterNumber);

    public static Lens<Chapter, IReadOnlyList<Page>> ChapterToPagesLens()
        => Lens<Chapter, IReadOnlyList<Page>>.Create(chapter => chapter.Pages);

    public static Lens<Chapter, Page> ChapterToPageLens(int pageNumber)
        => Lens<Chapter, Page>.Create<IReadOnlyList<Page>>(chapter => chapter.Pages, page => page.Number == pageNumber);
}
