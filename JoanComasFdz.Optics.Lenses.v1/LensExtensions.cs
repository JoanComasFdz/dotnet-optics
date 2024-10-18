namespace JoanComasFdz.Optics.Lenses.v1;

public static class LensExtensions
{
    public static Lens<TWhole, TSubPart> Compose<TWhole, TPart, TSubPart>(
        this Lens<TWhole, TPart> parent, Lens<TPart, TSubPart> child)
        => new(
          whole => child.Get(parent.Get(whole)),
          (whole, part) => parent.Set(whole, child.Set(parent.Get(whole), part))
          );

    public static TWhole Update<TWhole, TPart>(this Lens<TWhole, TPart> lens, TWhole whole, Func<TPart, TPart> transform)
    {
        var part = lens.Get(whole);
        var updatedPart = transform(part);
        return lens.Set(whole, updatedPart);
    }
}