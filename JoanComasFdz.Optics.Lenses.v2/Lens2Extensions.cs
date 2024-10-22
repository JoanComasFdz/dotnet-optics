namespace JoanComasFdz.Optics.Lenses.v2;

public static class LensExtensions
{
    public static Lens<TWhole, TSubPart> Compose<TWhole, TPart, TSubPart>(
    this Lens<TWhole, TPart> parent, Lens<TPart, TSubPart> child)
    => new(
      whole => child.Get(parent.Get(whole)),
      (whole, part) => parent.Set(whole, child.Set(parent.Get(whole), part))
      );

    public static TWhole Update<TWhole, TPart>(this Lens<TWhole, TPart> lens2, TWhole whole, Func<TPart, TPart> updateFunc)
    {
        var currentPart = lens2.Get(whole);
        var updatedPart = updateFunc(currentPart);
        return lens2.Set(whole, updatedPart);
    }
}
