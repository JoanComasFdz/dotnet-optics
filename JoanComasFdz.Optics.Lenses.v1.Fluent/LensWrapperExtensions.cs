namespace JoanComasFdz.Optics.Lenses.v1.Fluent;

public static class LensWrapperExtensions
{
    public static TWhole With<TWhole, TPart>(this LensWrapper<TWhole, TPart> wrapper, Func<TPart, TPart> transform)
        => wrapper.Lens.Update(wrapper.Whole, transform);

}