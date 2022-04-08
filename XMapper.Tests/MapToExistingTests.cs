using Xunit;

namespace XMapper.Tests;
public class MapToExistingTests
{
    [Theory]
    [InlineData("X1", 3, 5, "X2", DummyEnum.None, DummyEnum.One)]
    [InlineData("", null, 5, null, DummyEnum.One, null)]
    public void MapAllToExisting_SourceList(string s, int? ni, int i, string? ns, DummyEnum e, DummyEnum? ne)
    {
        var source = new Dummy1 { XString = s, XNullableInt = ni, XInt = i, XNullableString = ns, XEnum = e, XNullableEnum = ne };
        var target = new Dummy2();

        var mapper = new XMapper<Dummy1, Dummy2>(UsePropertyListOf.Source);
        mapper.Map(source, target);

        Assert.Equal(s, target.XString);
        Assert.Equal(ni, target.XNullableInt);
        Assert.Equal(i, target.XInt);
        Assert.Equal(ns, target.XNullableString);
        Assert.Equal(e, target.XEnum);
        Assert.Equal(ne, target.XNullableEnum);
    }

    [Theory]
    [InlineData("X1", 3, 5, "X2", DummyEnum.None, DummyEnum.One)]
    [InlineData("", null, 5, null, DummyEnum.One, null)]
    public void MapAllToExisting_TargetList(string s, int? ni, int i, string? ns, DummyEnum e, DummyEnum? ne)
    {
        var source = new Dummy1 { XString = s, XNullableInt = ni, XInt = i, XNullableString = ns, XEnum = e, XNullableEnum = ne };
        var target = new Dummy2();

        var mapper = new XMapper<Dummy1, Dummy2>(UsePropertyListOf.Target)
            .IgnoreTargetProperty(x => x.XNullableString2);
        mapper.Map(source, target);

        Assert.Equal(s, target.XString);
        Assert.Equal(ni, target.XNullableInt);
        Assert.Equal(i, target.XInt);
        Assert.Equal(ns, target.XNullableString);
        Assert.Equal(e, target.XEnum);
        Assert.Null(target.XNullableString2);
    }
}
