namespace JoanComasFdz.Optics.Lenses;

public static class OldLensExtensions
{
    public static OldLens<TWhole, TSubPart> Compose<TWhole, TPart, TSubPart>(
        this OldLens<TWhole, TPart> parent, OldLens<TPart, TSubPart> child)
        => new(
          whole => child.Get(parent.Get(whole)),
          (whole, part) => parent.Set(whole, child.Set(parent.Get(whole), part))
          );

    public static TWhole Mutate<TWhole, TPart>(this OldLens<TWhole, TPart> lens, TWhole whole, Func<TPart, TPart> transform)
    {
        var part = lens.Get(whole);
        var updatedPart = transform(part);
        return lens.Set(whole, updatedPart);
    }

    public static TWhole With<TWhole, TPart>(this TWhole whole, OldLens<TWhole, TPart> lens, Func<TPart, TPart> transform)
    {
        return lens.Mutate(whole, transform);
    }
}