using Xunit;

namespace XMapper.Tests;

public class MapToNewTests
{
    [Theory]
    [InlineData("X1", 3, 5, "X2", DummyEnum.None, DummyEnum.One)]
    [InlineData("", null, 0, null, DummyEnum.One, null)]
    public void MapAllToNew_SourceList(string s, int? ni, int i, string? ns, DummyEnum e, DummyEnum? ne)
    {
        var mapper = new XMapper<Dummy1, Dummy2>(UsePropertyListOf.Source);
        var dummy2 = mapper.Map(new Dummy1 { XString = s, XNullableInt = ni, XInt = i, XNullableString = ns, XEnum = e, XNullableEnum = ne });
        Assert.Equal(s, dummy2.XString);
        Assert.Equal(ni, dummy2.XNullableInt);
        Assert.Equal(i, dummy2.XInt);
        Assert.Equal(ns, dummy2.XNullableString);
        Assert.Equal(e, dummy2.XEnum);
        Assert.Equal(ne, dummy2.XNullableEnum);
    }

    [Theory]
    [InlineData("X1", 3, 5, "X2", DummyEnum.None, DummyEnum.One)]
    [InlineData("", null, 0, null, DummyEnum.One, null)]
    public void MapAllToNew_TargetList(string s, int? ni, int i, string? ns, DummyEnum e, DummyEnum? ne)
    {
        var mapper = new XMapper<Dummy1, Dummy2>(UsePropertyListOf.Target)
            .IgnoreTargetProperty(x => x.XNullableString2);
        var dummy2 = mapper.Map(new Dummy1 { XString = s, XNullableInt = ni, XInt = i, XNullableString = ns, XEnum = e, XNullableEnum = ne });
        Assert.Equal(s, dummy2.XString);
        Assert.Equal(ni, dummy2.XNullableInt);
        Assert.Equal(i, dummy2.XInt);
        Assert.Equal(ns, dummy2.XNullableString);
        Assert.Equal(e, dummy2.XEnum);
        Assert.Equal(ne, dummy2.XNullableEnum);
    }
}
