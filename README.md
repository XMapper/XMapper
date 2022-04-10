# XMapper

The object-to-object mapper for C# that can be setup in a readable one-liner. No Attributes, no configuration - just write intuitive and safe code that works immediately.

## Examples

### Map to new
```csharp
var dummy1 = new Dummy1 { ... };

var mapper = new XMapper<Dummy1, Dummy2>(PropertyList.Source);

var dummy2 = mapper.Map(dummy1);
```

### Map to exising
```csharp
var d1 = new Dummy1 { ... };
var d2 = new Dummy2 { ... };

var mapper = new XMapper<Dummy1, Dummy2>(PropertyList.Target)
    .IgnoreTargetProperty(x => x.Id);

mapper.Map(d1, d2);
```

### Map Enumerable members
Enumerable with refernce type elements:
```csharp
var d1Xd2 = new XMapper<Dummy1, Dummy2>(PropertyList.Source);
var mapper = new XMapper<DummyA, DummyB>(PropertyList.Source)
    .IncludeAction((source, target) => target.Dummy2List = source.Dummy1Array?.Select(x => d1Xd2.Map(x)).ToList());
```
Enumerable with ValueType elements:
```csharp
var mapper = new XMapper<DummyA, DummyB>(PropertyList.Source)
    .IncludeAction((source, target) => target.XIntList = source.XIntArray?.ToList());
```

### Map non-enumerable reference type members
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
var dummy2 = dummyB.Dummy2Property
```

### Custom mapping of ValueTypes
```csharp
var d1 = new Dummy1 { ... };
var d2 = new Dummy2 { ... };

var mapper = new XMapper<Dummy1, Dummy2>(PropertyList.Source)
    .IgnoreSourceProperty(x => x.MyInt)
    .IncludeAction((source, target) => target.MyInt = target.MyEnum > someValue ? null : target.MyInt = source.Number * 10);

mapper.Map(d1, d2);
```
