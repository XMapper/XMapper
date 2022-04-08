using System.Linq;
using Xunit;

namespace XMapper.Tests;

public class InitialPropertyListTests
{
    [Fact]
    public void SourceProperties_Dummy1()
    {
        var mapper = new XMapper<Dummy1, Dummy2>(UsePropertyListOf.Source);
        var propertyNames = mapper._propertyInfos.OrderBy(x => x.Name).Select(x => x.Name);

        Assert.Equal(new[]
        {
            nameof(Dummy1.XEnum),
            nameof(Dummy1.XInt),
            nameof(Dummy1.XNullableEnum),
            nameof(Dummy1.XNullableInt),
            nameof(Dummy1.XNullableString),
            nameof(Dummy1.XString),
        }, propertyNames);
    }

    [Fact]
    public void SourceProperties_Dummy3()
    {
        var mapper = new XMapper<Dummy3, Dummy2>(UsePropertyListOf.Source);
        var propertyInfo = Assert.Single(mapper._propertyInfos);
        Assert.Equal(nameof(Dummy3.XNullableString), propertyInfo.Name);
    }

    [Fact]
    public void TargetProperties_Dummy2()
    {
        var mapper = new XMapper<Dummy1, Dummy2>(UsePropertyListOf.Target);
        var propertyNames = mapper._propertyInfos.OrderBy(x => x.Name).Select(x => x.Name);

        Assert.Equal(new[]
        {
            nameof(Dummy2.XEnum),
            nameof(Dummy2.XInt),
            nameof(Dummy2.XNullableEnum),
            nameof(Dummy2.XNullableInt),
            nameof(Dummy2.XNullableString),
            nameof(Dummy2.XNullableString2),
            nameof(Dummy2.XString),
        }, propertyNames);
    }

    [Fact]
    public void TargetProperties_Dummy3()
    {
        var mapper = new XMapper<Dummy1, Dummy3>(UsePropertyListOf.Target);
        var propertyInfo = Assert.Single(mapper._propertyInfos);
        Assert.Equal(nameof(Dummy3.XNullableString), propertyInfo.Name);
    }
}
