using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace XMapper.Tests;

public class Tests
{
    public class Dummy1
    {
        public string XString { get; set; } = "";
        public string? XNullableString { get; set; }
        public int XInt { get; set; }
        public int? XNullableInt { get; set; }
        public DummyEnum XEnum { get; set; }

        public Type? XType { get; set; }
        public IEnumerable<int> Multiple { get; set; } = Enumerable.Empty<int>();
    }

    public class Dummy2
    {
        public string XString { get; set; } = "";
        public string? XNullableString { get; set; }
        public int XInt { get; set; }
        public int? XNullableInt { get; set; }
        public DummyEnum XEnum { get; set; }
    }

    public enum DummyEnum
    {
        None,
        One,
    }

    [Fact]
    public void SourceProperties()
    {
        var mapper = new XMapper<Dummy1, Dummy2>();
        var expected = new[]
        {
            nameof(Dummy1.XEnum),
            nameof(Dummy1.XInt), 
            nameof(Dummy1.XNullableInt), 
            nameof(Dummy1.XNullableString), 
            nameof(Dummy1.XString), 
        };
        var actual = mapper.SourceProperties.OrderBy(x => x.Name).Select(x => x.Name);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Ignore()
    {
        var mapper = new XMapper<Dummy1, Dummy2>()
            .Ignore(x => x.XNullableInt)
            .Ignore(x => x.XString)
            .Ignore(x => x.XEnum);
        var expected = new[] { nameof(Dummy1.XInt), nameof(Dummy1.XNullableString) };
        var actual = mapper.SourceProperties.OrderBy(x => x.Name).Select(x => x.Name);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("X1", 3, 5, "X2", DummyEnum.None)]
    [InlineData("", null, 0, null, DummyEnum.One)]
    public void Map_all_to_new(string s, int? ni, int i, string? ns, DummyEnum e)
    {
        var mapper = new XMapper<Dummy1, Dummy2>();
        var result = mapper.Map(new Dummy1 { XString = s, XNullableInt = ni, XInt = i, XNullableString = ns, XEnum = e });
        Assert.Equal(s, result.XString);
        Assert.Equal(ni, result.XNullableInt);
        Assert.Equal(i, result.XInt);
        Assert.Equal(ns, result.XNullableString);
        Assert.Equal(e, result.XEnum);
    }

    [Theory]
    [InlineData("X1", 3, 5, "X2", DummyEnum.None)]
    [InlineData("", null, 5, null, DummyEnum.One)]
    public void Map_all_to_existing(string s, int? ni, int i, string? ns, DummyEnum e)
    {
        var mapper = new XMapper<Dummy1, Dummy2>();
        var target = new Dummy2();
        mapper.Map(new Dummy1 { XString = s, XNullableInt = ni, XInt = i, XNullableString = ns, XEnum = e }, target);
        Assert.Equal(s, target.XString);
        Assert.Equal(ni, target.XNullableInt);
        Assert.Equal(i, target.XInt);
        Assert.Equal(ns, target.XNullableString);
    }
}
