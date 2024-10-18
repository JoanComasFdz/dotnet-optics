namespace JoanComasFdz.Optics.Lenses.v1;

/// <summary>
/// Ssimplifies access and modification of deep parts of an immutable object).
/// Should be used via <see cref="LensExtensions"/>.
/// </summary>
/// <remarks>
/// <seealso href="https://medium.com/@gcanti/introduction-to-optics-lenses-and-prisms-3230e73bfcfe"/>
/// <seealso href="https://github.com/dotnet/csharplang/issues/302"/>
/// <seealso href="https://gist.github.com/dadhi/3db1ed45a60bceaa16d051ee9a4ab1b7"/>
public record Lens<TWhole, TPart>(Func<TWhole, TPart> Get, Func<TWhole, TPart, TWhole> Set);