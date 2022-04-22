## [3.0.2] 2022-04-22

- Also provide property name(s) in case of a type mismatch exception: `'DummyA.P' is of type 'System.String', but 'DummyB.P' is of type 'System.Int32'.` (instead of `Object of type 'System.String' cannot be converted to type 'System.Int32'`, which did not point to the property name).

## [3.0.1] 2022-04-20

- Improve source code summaries.

## [3.0.0] 2022-04-17

### Breaking changes

- All properties are mapped automatically by name, even reference types. So migrating from 2.0 to 3.0 may require adding a few calls to `IgnoreSourceProperty` or `IgnoreTargetProperty`. In case the target and source member have the same type, the reference is passed on.
- Trying to map null to a not-nullable reference type results in an error. This a an extra safety check that is probably welcome.
- .NET 6 is required instead of .NET 5, so that `NullabilityInfoContext` can be used for (nullable) reference types.

## [2.0.1] 2022-04-16

- Improvements in the documentation that is available on hovering over the `XMapper` class or its methods.
- Better error messages.

## [2.0.0] 2022-04-14

### Changed

- Trying to assign `null` to a not-nullable `ValueType` now throws an exception instead of setting the default value of the underlying type.
- TSource now has the constraints `class` and `new()`. Requiring `new()` makes automated testing via XMapper.Testing possible.
- The readonly field `_propertyList` is now private, as it should have been already.
