# Description
This folder showcases how to use Lenses for a particular domain.

`LibraryLenses.cs` provides the methods necessary to create lenses for a `Library`.

For collection properties, there are two sets of methods:
- Focus on the collection itself (`LibraryToBooksLens`, notice that _Books_ is plural): this allows to create a new instance of `Library` with changes in the focused collection (add, remove, filter, etc).
- Focus on an item inside the collection (`LibraryToBookLens`, notice that _Book_ is singular): this allows to create a new instance of `Library` with changes in one specific item.

For the second set of methods, they receive a predicate, so that the caller can specify how the item is filtered / selected.

`CasesUsingHardcodedLenses.cs` contains a set of methods with real world examples of how a developer should use the lenses.