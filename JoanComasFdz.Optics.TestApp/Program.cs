using JoanComasFdz.Optics.TestApp.UsingEnhancedApi;
using JoanComasFdz.Optics.TestApp.UsingHardcodedLenses;
using JoanComasFdz.Optics.TestApp.UsingWiths;
using Optics.TestApp.Domain;
using System.Text.Json;

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

Console.WriteLine("===  ADDING ITEMS ===");

Console.WriteLine("Add a new book to the library");
var l1 = CasesUsingJustWith.AddBookToLibrary(library);
var l2 = CasesUsingHardcodedLenses.AddBookToLibrary(library);
var l3 = CasesUsingEnhancedApi.AddBookToLibrary(library);
AssertEqual(l1, l2);
AssertEqual(l1, l3);

Console.WriteLine("Add a chapter to the book 1234");
l1 = CasesUsingJustWith.AddChapterToBook(library, "1234");
l2 = CasesUsingHardcodedLenses.AddChapterToBook(library, "1234");
l3 = CasesUsingEnhancedApi.AddChapterToBook(library, "1234");
AssertEqual(l1, l2);
AssertEqual(l2, l3);

Console.WriteLine("Add a page to the chapter 2 of the book 1234");
l1 = CasesUsingJustWith.AddPageToChapterOfBook(library, "1234", 1);
l2 = CasesUsingHardcodedLenses.AddPageToChapterOfBook(library, "1234", 1);
l3 = CasesUsingEnhancedApi.AddPageToChapterOfBook(library, "1234", 1);
AssertEqual(l1, l2);
AssertEqual(l2, l3);

Console.WriteLine("===  UPDATING ITEMS ===");

Console.WriteLine("Update title of book 1234.");
l1 = CasesUsingJustWith.UpdateBookTitle(library, "1234", "Program nicely");
l2 = CasesUsingHardcodedLenses.UpdateBookTitle(library, "1234", "Program nicely");
l3 = CasesUsingEnhancedApi.UpdateBookTitle(library, "1234", "Program nicely");
AssertEqual(l1, l2);
AssertEqual(l2, l3);

Console.WriteLine("Update title of chapter 1 of book 1234.");
l1 = CasesUsingJustWith.UpdateChapterTitleOfBook(library, "1234", 1, "Easy Introduction");
l2 = CasesUsingHardcodedLenses.UpdateChapterTitleOfBook(library, "1234", 1, "Easy Introduction");
l3 = CasesUsingEnhancedApi.UpdateChapterTitleOfBook(library, "1234", 1, "Easy Introduction");
AssertEqual(l1, l2);
AssertEqual(l2, l3);

Console.WriteLine("Update the page content of page 1 of chapter 1 of book 1234.");
l1 = CasesUsingJustWith.UpdatePageContentOfChapterOfBook(library, "1234", 1, 1, "New page content!");
l2 = CasesUsingHardcodedLenses.UpdatePageContentOfChapterOfBook(library, "1234", 1, 1, "New page content!");
l3 = CasesUsingEnhancedApi.UpdatePageContentOfChapterOfBook(library, "1234", 1, 1, "New page content!");
AssertEqual(l1, l2);
AssertEqual(l2, l3);



static void AssertEqual(Library l1, Library l2)
{
    var l1Json = JsonSerializer.Serialize(l1);
    var l2Json = JsonSerializer.Serialize(l2);

    if (l1Json.Equals(l2Json)) return;

    Console.WriteLine($"ERROR - Libraries are different:{Environment.NewLine}{l1Json}{Environment.NewLine}{Environment.NewLine}{l2Json}{Environment.NewLine}");
}