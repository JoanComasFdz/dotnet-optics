using JoanComasFdz.Optics.Lenses;
using JoanComasFdz.Optics.TestApp.Domain;

namespace JoanComasFdz.Optics.TestApp.UsingHardcodedLens2;

public static class LibraryLenses2
{
    public static Lens2<Library, Address> LibraryToAddres()
        => Lens2<Library, Address>.Create(library => library.Address);

    public static Lens2<Library, IReadOnlyList<Book>> LibraryToBooksLens()
        => Lens2<Library, IReadOnlyList<Book>>.Create(library => library.Books);

    public static Lens2<Library, Book> LibraryToBookLens(string bookISDN)
        => Lens2<Library, Book>.Create<IReadOnlyCollection<Book>>(library => library.Books, book => book.ISDN == bookISDN);

    public static Lens2<Book, IReadOnlyList<Chapter>> BookToChaptersLens()
        => Lens2<Book, IReadOnlyList<Chapter>>.Create(book => book.Chapters);

    public static Lens2<Book, Chapter> BookToChapterLens(int chapterNumber)
        => Lens2<Book, Chapter>.Create<IReadOnlyList<Chapter>>(book => book.Chapters, chapter => chapter.Number == chapterNumber);

    public static Lens2<Chapter, IReadOnlyList<Page>> ChapterToPagesLens()
        => Lens2<Chapter, IReadOnlyList<Page>>.Create(chapter => chapter.Pages);

    public static Lens2<Chapter, Page> ChapterToPageLens(int pageNumber)
        => Lens2<Chapter, Page>.Create<IReadOnlyList<Page>>(chapter => chapter.Pages, page => page.Number == pageNumber);
}
