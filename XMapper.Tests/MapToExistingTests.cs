using Xunit;

namespace XMapper.Tests;
public class MapToExistingTests
{
    [Theory]
    [InlineData("X1", 3, 5, "X2", DummyEnum.None, DummyEnum.One)]
    [InlineData("", null, 5, null, DummyEnum.One, null)]
    public void MapAllToExisting_SourceList(string s, int? ni, int i, string? ns, DummyEnum e, DummyEnum? ne)
    {
        var d1 = new Dummy1 { XString = s, XNullableInt = ni, XInt = i, XNullableString = ns, XEnum = e, XNullableEnum = ne };
        var d2 = new Dummy2();

        var mapper = new XMapper<Dummy1, Dummy2>(PropertyList.Source);
        mapper.Map(d1, d2);

        Assert.Equal(s, d2.XString);
        Assert.Equal(ni, d2.XNullableInt);
        Assert.Equal(i, d2.XInt);
        Assert.Equal(ns, d2.XNullableString);
        Assert.Equal(e, d2.XEnum);
        Assert.Equal(ne, d2.XNullableEnum);
    }

    [Theory]
    [InlineData("X1", 3, 5, "X2", DummyEnum.None, DummyEnum.One)]
    [InlineData("", null, 5, null, DummyEnum.One, null)]
    public void MapAllToExisting_TargetList(string s, int? ni, int i, string? ns, DummyEnum e, DummyEnum? ne)
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
    }
}
