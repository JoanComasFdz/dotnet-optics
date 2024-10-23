using JoanComasFdz.Optics.TestApp.Domain;
using System.Text.Json;
using JoanComasFdz.Optics.TestApp.HowToUse.v1;
using JoanComasFdz.Optics.TestApp.HowToUse.v1.Simplified;
using JoanComasFdz.Optics.TestApp.HowToUse.v1.Fluent;
using JoanComasFdz.Optics.TestApp.HowToUse.v1.FluentSimplified;

Console.WriteLine("Creating an initial library");
var library = new Library(
    Name: "Downtown Public Library",
    Address: new Address(
        Street: "456 Oak Street",
        City: "Metropolis",
        PostalCode: "67890"
    ),
    Books: [
        new Book(
            ISDN: "1234",
            Title: "The Art of Programming",
            Author: "Alice Smith",
            Chapters: [
                new Chapter(
                    Number: 1,
                    Title: "Introduction to Algorithms",
                    Pages: [
                        new Page(1, "Page 1 Content")
                    ]
                )]
        )
    ]
);

Console.WriteLine("=== ADDING ITEMS ===");

Console.WriteLine("Add a new book to the library");
var l4 = CasesUsingHardcodedLensesFluent.AddBookToLibrary(library);
var l5 = CasesUsingHardcodedLensesFluentSimplified.AddBookToLibrary(library);

AssertEqual(l4, l5);

Console.WriteLine("Add a chapter to the book 1234");
l4 = CasesUsingHardcodedLensesFluent.AddChapterToBook(library, "1234");
l5 = CasesUsingHardcodedLensesFluentSimplified.AddChapterToBook(library, "1234");

AssertEqual(l4, l5);

Console.WriteLine("Add a page to the chapter 2 of the book 1234");
l4 = CasesUsingHardcodedLensesFluent.AddPageToChapterOfBook(library, "1234", 1);
AssertEqual(l4, l5);


Console.WriteLine("=== UPDATING ITEMS ===");

Console.WriteLine("Update title of book 1234.");
l4 = CasesUsingHardcodedLensesFluent.UpdateBookTitle(library, "1234", "Program nicely");
l5 = CasesUsingHardcodedLensesFluentSimplified.UpdateBookTitle(library, "1234", "Program nicely");
AssertEqual(l4, l5);

Console.WriteLine("Update title of chapter 1 of book 1234.");
l4 = CasesUsingHardcodedLensesFluent.UpdateChapterTitleOfBook(library, "1234", 1, "Easy Introduction");
l5 = CasesUsingHardcodedLensesFluentSimplified.UpdateChapterTitleOfBook(library, "1234", 1, "Easy Introduction");
AssertEqual(l4, l5);

Console.WriteLine("Update the page content of page 1 of chapter 1 of book 1234.");
l4 = CasesUsingHardcodedLensesFluent.UpdatePageContentOfChapterOfBook(library, "1234", 1, 1, "New page content!");
l5 = CasesUsingHardcodedLensesFluentSimplified.UpdatePageContentOfChapterOfBook(library, "1234", 1, 1, "New page content!");
AssertEqual(l4, l5);


Console.WriteLine("=== REMOVING ITEMS ===");

Console.WriteLine("Remove the book 1234 from the library.");
l4 = CasesUsingHardcodedLensesFluent.RemoveBookFromLibrary(library, "1234");
l5 = CasesUsingHardcodedLensesFluentSimplified.RemoveBookFromLibrary(library, "1234");
AssertEqual(l4, l5);

Console.WriteLine("Remove the chapter 1 of the book 1234 from the library.");
l4 = CasesUsingHardcodedLensesFluent.RemoveChapterFromBook(library, "1234", 1);
l5 = CasesUsingHardcodedLensesFluentSimplified.RemoveChapterFromBook(library, "1234", 1);
AssertEqual(l4, l5);

Console.WriteLine("Remove the page 1 of the chapter 1 of the book 1234 from the library.");
l4 = CasesUsingHardcodedLensesFluent.RemovePageFromChapterOfBook(library, "1234", 1, 1);
l5 = CasesUsingHardcodedLensesFluentSimplified.RemovePageFromChapterOfBook(library, "1234", 1, 1);
AssertEqual(l4, l5);

static void AssertEqual(Library l1, Library l2)
{
    var l1Json = JsonSerializer.Serialize(l1);
    var l2Json = JsonSerializer.Serialize(l2);

    if (l1Json.Equals(l2Json)) return;

    Console.WriteLine($"ERROR - Libraries are different:{Environment.NewLine}{l1Json}{Environment.NewLine}{Environment.NewLine}{l2Json}{Environment.NewLine}");
}