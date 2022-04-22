# XMapper

The object-to-object mapper for C# that can be setup in a readable one-liner. No Attributes, no configuration - just write intuitive and safe code that works immediately.
<p align="center">
    <img src="https://avatars.githubusercontent.com/u/103217522?s=150&v=4" alt="XMapper logo"/>
</p>

Available via [NuGet](https://www.nuget.org/packages/XMapper).

For testing of all your object-to-object mappers with one line of unit test code, see [XMapper.Testing](https://github.com/XMapper/XMapper.Testing).

## Introduction
There's only one class in this package: `XMapper<TSource, TTarget>`. It has a single parameter of type `PropertyList`.

These are all the public methods:

- `IgnoreSourceProperty`
- `IgnoreTargetProperty`
- `IncludeAction`
- `Map` (2 overloads)

Hovering over `XMapper` and its methods in your editor will provide guiding documentation.

## Examples
```csharp
using XMapper;
```

### Map to new
```csharp
var dummy1 = new Dummy1
{
    MyString = "All members with the same name and type can be copied automatically...",
    MyIntArray = new [] { 2, 3, 5 },
    MyObjectList = new List<MyObject> { new MyObject { Hello = "...even a list of objects!" }, new MyObject() },
};

var mapper = new XMapper<Dummy1, Dummy2>(PropertyList.Source);

var dummy2 = mapper.Map(dummy1);
```

### Map to exising
```csharp
var d1 = new Dummy1 { SomeString = "I will be copied", Id = "I won't be copied" };
var d2 = new Dummy2 { SomeString = "I will be overwritten", Id = "I will stay" };

var mapper = new XMapper<Dummy1, Dummy2>(PropertyList.Target)
    .IgnoreTargetProperty(x => x.Id);

mapper.Map(d1, d2);
```

### Custom mapping of ValueTypes
```csharp
var d1 = new Dummy1 { ... };
var d2 = new Dummy2 { ... };

var mapper = new XMapper<Dummy1, Dummy2>(PropertyList.Source)
    .IgnoreSourceProperty(x => x.Number)
    .IncludeAction((source, target) =>
    {
        if (target.MyInt != "An important value")
        {
            target.MyInt = source.Number * 10);
        }
    });

mapper.Map(d1, d2);
```

### Map enumerable members to another target type
From Array to Array with differently reference-typed elements:
```csharp
var d1Xd2 = new XMapper<Dummy1, Dummy2>(PropertyList.Source);
var mapper = new XMapper<DummyA, DummyB>(PropertyList.Source)
    .IgnoreSourceProperty(x => x.Dummy1Array)
    .IncludeAction((source, target) => target.Dummy2Array = source.Dummy1Array?.Select(x => d1Xd2.Map(x)).ToArray());
```
From Array to List:
```csharp
var mapper = new XMapper<DummyA, DummyB>(PropertyList.Source)
    .IgnoreSourceProperty(x => x.XIntArray)
    .IncludeAction((source, target) => target.XIntList = source.XIntArray?.ToList());
```

### Map non-enumerable reference-typed members to another target type
```csharp
var dummyA = new DummyA
{
    Dummy1Property = new Dummy1
    {
        ...
    },
    ...
};

var d1Xd2 = new XMapper<Dummy1, Dummy2>(PropertyList.Source);
var mapper = new XMapper<DummyA, DummyB>(PropertyList.Target)
    .IgnoreTargetProperty(x => x.Dummy2Property)
    .IncludeAction((dummyA, dummyB) =>
    {
        if (dummyA.Dummy1Property == null)
        {
            dummyB.Dummy2Property = null;
        }
        else
        {
            d1Xd2.Map(dummyA.Dummy1Property, dummyB.Dummy2Property ??= new());
        }
    });

var dummyB = mapper.Map(dummyA);
var dummy2 = dummyB.Dummy2Property;
```

### Map just a few properties from a reference-typed member
```csharp
var dummyA = new DummyA
{
    ...
    Dummy1Property = new Dummy1
    {
        Name = "Not DummyA, but Dummy1",
        Address = "Deep",
        ...
    },
    ...
};

var mapper = new XMapper<DummyA, DummyB>(PropertyList.Target)
    .IgnoreTargetProperty(x => x.Name)
    .IgnoreTargetProperty(x => x.Address)
    .IncludeAction((source, target) =>
    {
        target.Name = source.Dummy1Property.Name;
        target.Address = source.Dummy1Property.Address;
    });

var dummyB = mapper.Map(dummyA);
var name = dummyB.Name;

```