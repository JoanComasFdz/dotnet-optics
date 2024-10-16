﻿using JoanComasFdz.Optics.TestApp.Domain;
using JoanComasFdz.Optics.TestApp.UsingEnhancedApi;

namespace JoanComasFdz.Optics.TestApp.UsingEnhancedApiSimplified;

public static class CasesUsingEnhancedApiSimplified
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

        return library
            .BooksLens()
            .With(books => [.. books, secondBook]);
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

        return library
            .BookLens(bookISDN)
            .ChaptersLens()
            .With(chapters => [.. chapters, secondChapter]);
    }

    public static Library AddPageToChapterOfBook(Library library, string bookISDN, int chapterNumber)
    {
        return library
            .BookLens(bookISDN)
            .ChapterLens(chapterNumber)
            .PagesLens()
            .With(pages => [.. pages, new Page(2, "Page 2 Content")]);
    }

    public static Library UpdateBookTitle(Library library, string bookISDN, string newTitle)
    {
        return library
            .BookLens(bookISDN)
            .With(book => book with { Title = newTitle });
    }

    public static Library UpdateChapterTitleOfBook(Library library, string bookISDN, int chapterNumber, string newTitle)
    {
        return library
            .BookLens(bookISDN)
            .ChapterLens(chapterNumber)
            .With(chapter => chapter with { Title = newTitle });
    }

    public static Library UpdatePageContentOfChapterOfBook(Library library, string bookISDN, int chapterNumber, int pageNumber, string newContent)
    {
        return library
            .BookLens(bookISDN)
            .ChapterLens(chapterNumber)
            .PageLens(pageNumber)
            .With(page => page with { Content = newContent });
    }

    public static Library RemoveBookFromLibrary(Library library, string bookISDN)
    {
        return library.BooksLens().With(books => books.Where(book => book.ISDN != bookISDN).ToArray());
    }

    public static Library RemoveChapterFromBook(Library library, string bookISDN, int chapterNumber)
    {
        return library.BookLens(bookISDN)
            .ChaptersLens()
            .With(chapers => chapers.Where(chapter => chapter.Number != chapterNumber).ToArray());
    }

    public static Library RemovePageFromChapterOfBook(Library library, string bookISDN, int chapterNumber, int pageNumber)
    {
        return library.BookLens(bookISDN)
            .ChapterLens(chapterNumber)
            .PagesLens()
            .With(pages => pages.Where(page =>  page.Number != pageNumber).ToArray());
    }
}