namespace JoanComasFdz.Optics.TestApp.Domain;

public record Library(string Name, Address Address, IReadOnlyList<Book> Books);
public record Address(string Street, string City, string PostalCode);
public record Book(string ISDN, string Title, string Author, IReadOnlyList<Chapter> Chapters);
public record Chapter(int Number, string Title, IReadOnlyList<Page> Pages);
public record Page(int Number, string Content);
