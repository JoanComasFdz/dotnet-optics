namespace JoanComasFdz.Optics.Lenses.v1.Fluent;

/// <summary>
/// Stores the instance to which a lens will be applied, and the lens to be applied.
/// </summary>
/// <typeparam name="TWhole"></typeparam>
/// <typeparam name="TPart"></typeparam>
/// <param name="Whole"></param>
/// <param name="Lens"></param>
public record LensWrapper<TWhole, TPart>(TWhole Whole, Lens<TWhole, TPart> Lens);
