namespace JoanComasFdz.Optics.Lenses.v1.Fluent;

public static class LensWrapperExtensions
{
    /// <summary>
    /// Applies the specified transormation to a <see cref="LensWrapper{TWhole, TPart}"/>. This enables a fluent syntax when using the fluent
    /// apporach with <see cref="LensWrapper{TWhole, TPart}"/>, like:
    /// <code>
    /// var newLibrary = library
    ///        .BookLens(book => book.ISDN == bookISDN)       // This returns a LensWrapper.
    ///        .With(book => book with { Title = newTitle }); // This method.
    /// </code>
    /// <para>
    /// Typical usages include:
    /// <list type="bullet">
    /// <item>Edit a value: <c>.With(book => book with { Title = newTitle });</c></item>
    /// <item>Add an item to a collection: <c>.With(books => [.. books, secondBook]);</c></item>
    /// <item>Remove an item from a collection: <c>.With(books => books.Where(book => book.ISDN != bookISDN).ToArray());</c></item>
    /// </list>
    /// </para>
    /// </summary>
    /// <typeparam name="TWhole"></typeparam>
    /// <typeparam name="TPart"></typeparam>
    /// <param name="wrapper"></param>
    /// <param name="transform"></param>
    /// <returns></returns>
    public static TWhole With<TWhole, TPart>(this LensWrapper<TWhole, TPart> wrapper, Func<TPart, TPart> transform)
        => wrapper.Lens.Update(wrapper.Whole, transform);
}