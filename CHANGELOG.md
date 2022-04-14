## [2.0.0] 2022-04-14

### Changed

- Trying to assign `null` to a not-nullable `ValueType` now throws an exception instead of setting the default value of the underlying type.
- TSource now has the constraints `class` and `new()`. Requiring `new()` makes automated testing via XMapper.Testing possible.
- The readonly field `_propertyList` is now private, as it should have been already.
