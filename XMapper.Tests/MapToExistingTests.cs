using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace XMapper.Tests;
public class MapToExistingTests
{
    public class Dummy1
    {
        public string XString { get; set; } = "";
        public string? XNullableString { get; set; }
        public int XInt { get; set; }
        public int? XNullableInt { get; set; }
        public DummyEnum XEnum { get; set; }
        public DummyEnum? XNullableEnum { get; set; }


        public Type? XType { get; set; }
        public Type AnotherType { get; set; } = typeof(int);
        public IEnumerable<int> XEnumerable { get; set; } = Enumerable.Empty<int>();
        public int[] XIntArray { get; set; } = new[] { 2, 3, 5 };

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

        public Type? AnotherType { get; set; }
        public int[] XIntArray { get; set; } = Array.Empty<int>();
    }

    public enum DummyEnum
    {
        None,
        One,
    }

    [Theory]
    [InlineData("X1", 3, 5, "X2", DummyEnum.None, DummyEnum.One)]
    [InlineData("", null, 5, null, DummyEnum.One, null)]
    public void SourceList(string s, int? ni, int i, string? ns, DummyEnum e, DummyEnum? ne)
    {
        var d1 = new Dummy1 { XString = s, XNullableInt = ni, XInt = i, XNullableString = ns, XEnum = e, XNullableEnum = ne };
        var d2 = new Dummy2();

        var mapper = new XMapper<Dummy1, Dummy2>(PropertyList.Source)
            .IgnoreSourceProperty(x => x.XType)
            .IgnoreSourceProperty(x => x.XEnumerable);
        mapper.Map(d1, d2);

        Assert.Equal(s, d2.XString);
        Assert.Equal(ni, d2.XNullableInt);
        Assert.Equal(i, d2.XInt);
        Assert.Equal(ns, d2.XNullableString);
        Assert.Equal(e, d2.XEnum);
        Assert.Equal(ne, d2.XNullableEnum);
        Assert.Equal(typeof(int), d2.AnotherType);
        Assert.Equal(new[] { 2, 3, 5 }, d2.XIntArray);
    }

    [Theory]
    [InlineData("X1", 3, 5, "X2", DummyEnum.None, DummyEnum.One)]
    [InlineData("", null, 5, null, DummyEnum.One, null)]
    public void TargetList(string s, int? ni, int i, string? ns, DummyEnum e, DummyEnum? ne)
    {
        var d1 = new Dummy1 { XString = s, XNullableInt = ni, XInt = i, XNullableString = ns, XEnum = e, XNullableEnum = ne };
        var d2 = new Dummy2();

        var mapper = new XMapper<Dummy1, Dummy2>(PropertyList.Target)
            .IgnoreTargetProperty(x => x.XNullableString2);
        mapper.Map(d1, d2);

        Assert.Equal(s, d2.XString);
        Assert.Equal(ni, d2.XNullableInt);
        Assert.Equal(i, d2.XInt);
        Assert.Equal(ns, d2.XNullableString);
        Assert.Equal(e, d2.XEnum);
        Assert.Null(d2.XNullableString2);
        Assert.Equal(typeof(int), d2.AnotherType);
        Assert.Equal(new[] { 2, 3, 5 }, d2.XIntArray);
    }
}
