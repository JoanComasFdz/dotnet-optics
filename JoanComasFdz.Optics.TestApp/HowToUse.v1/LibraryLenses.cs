﻿using JoanComasFdz.Optics.Lenses;
using JoanComasFdz.Optics.TestApp.Domain;

namespace JoanComasFdz.Optics.TestApp.HowToUse.v1;

public static class LibraryLenses
{
    public static OldLens<Library, IReadOnlyList<Book>> LibraryToBooksLens() => new(
        library => library.Books,
        (library, newBooks) => library with { Books = newBooks }
    );

    public static OldLens<Library, Book> LibraryToBookLens(Func<Book, bool> predicate)
    {
        return new OldLens<Library, Book>(
            library => library.Books.Single(predicate),
            (library, updatedBook) => library with { Books = library.Books.Select(book => predicate(book) ? updatedBook : book).ToArray() }
        );
    }

    public static OldLens<Book, IReadOnlyList<Chapter>> BookToChaptersLens() => new(
        book => book.Chapters,
        (book, newChapters) => book with { Chapters = newChapters }
    );

    public static OldLens<Book, Chapter> BookToChapterLens(Func<Chapter, bool> predicate)
    {
        return new OldLens<Book, Chapter>(
            book => book.Chapters.Single(predicate),
            (book, updatedChapter) =>
            {
                var updatedChapters = book.Chapters.Select(chapter => predicate(chapter) ? updatedChapter : chapter).ToList();
                return book with { Chapters = updatedChapters.AsReadOnly() };
            }
        );
    }

    public static OldLens<Chapter, IReadOnlyList<Page>> ChapterToPagesLens() => new(
        chapter => chapter.Pages,
        (chapter, newPages) => chapter with { Pages = newPages }
    );

    public static OldLens<Chapter, Page> ChapterToPageLens(Func<Page, bool> predicate)
    {
        return new OldLens<Chapter, Page>(
            chapter => chapter.Pages.Single(predicate),
            (chapter, updatedPage) =>
            {
                var updatedPages = chapter.Pages.Select(page => predicate(page) ? updatedPage : page).ToList();
                return chapter with { Pages = updatedPages.AsReadOnly() };
            }
        );
    }
}