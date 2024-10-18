namespace JoanComasFdz.Optics.Lenses;

public static class Lens2Extensions
{
    public static Lens2<TWhole, TSubPart> Compose<TWhole, TPart, TSubPart>(
    this Lens2<TWhole, TPart> parent, Lens2<TPart, TSubPart> child)
    => new(
      whole => child.Get(parent.Get(whole)),
      (whole, part) => parent.Set(whole, child.Set(parent.Get(whole), part))
      );

    public static TWhole Update<TWhole, TPart>(this Lens2<TWhole, TPart> lens2, TWhole whole, Func<TPart, TPart> updateFunc)
    {
        var currentPart = lens2.Get(whole);
        var updatedPart = updateFunc(currentPart);
        return lens2.Set(whole, updatedPart);
    }
}
