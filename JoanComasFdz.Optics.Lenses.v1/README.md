# Lenses V1
The goal is to produce a the very first implementation of the `Lens` concept.

It provides the most basic infrastructure to construct a `Lens`.

The usage of this `Lens` is as follows:

- Declare a class with static methods with descriptive names that create the Lens on every call.
- Use a static import for the class, in the file where you need the lenses (not really necessary but is nicer).
- Use the `Compose()` method to chain lenses to go deeper into your type.
- Call `Update()` to create a new instance with all the same values except for the changes you need.