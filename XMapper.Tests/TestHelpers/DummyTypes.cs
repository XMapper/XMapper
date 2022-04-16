using System;
using System.Collections.Generic;
using System.Linq;

namespace XMapper.Tests;

public class Dummy1
{
    public string XString { get; set; } = "";
    public string? XNullableString { get; set; }
    public int XInt { get; set; }
    public int? XNullableInt { get; set; }
    public DummyEnum XEnum { get; set; }
    public DummyEnum? XNullableEnum { get; set; }


    public Type? XType { get; set; }
    public IEnumerable<int> Multiple { get; set; } = Enumerable.Empty<int>();
}

public class Dummy2
{
    public string XString { get; set; } = "";
    public string? XNullableString { get; set; }
    public string? XNullableString2 { get; set; }
    public int XInt { get; set; }
    public int? XNullableInt { get; set; }
    public DummyEnum XEnum { get; set; }
    public DummyEnum? XNullableEnum { get; set; }

}

public class Dummy3
{
    public string? XNullableString { get; set; }
}

public enum DummyEnum
{
    None,
    One,
}
