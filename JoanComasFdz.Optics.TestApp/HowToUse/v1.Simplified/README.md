# Description
This folder showcases how to use Lenses for a particular domain in a simplified way.

`LibraryLensesSimplified.cs` provides 3 new methods to create lenses for a `Library` to focus on single item in a collection property.

This methods no longer receive a predicate. Instead, the lens itself contains the logic about how the item is filtered / selected, and the caller only needs to specify the item id.

`CasesUsingHardcodedLensesSimplified.cs` contains a set of methods with real world examples of how a developer should use the lenses.