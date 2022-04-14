using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace XMapper.Tests;

public class IgnoreTests
{
    private static List<PropertyInfo> GetPrivatePropertyInfos(object mapper)
    {
        return (List<PropertyInfo>)mapper.GetType().GetRuntimeFields().First(x => x.Name == "_propertyInfos").GetValue(mapper)!;
    }

    [Fact]
    public void IgnoreSourceProperty()
    {
        var mapper = new XMapper<Dummy1, Dummy2>(PropertyList.Source)
            .IgnoreSourceProperty(x => x.XNullableInt)
            .IgnoreSourceProperty(x => x.XString)
            .IgnoreSourceProperty(x => x.XEnum)
            .IgnoreSourceProperty(x => x.XNullableEnum);
        var propertyNames = GetPrivatePropertyInfos(mapper).OrderBy(x => x.Name).Select(x => x.Name);

        Assert.Equal(new[]
        {
            nameof(Dummy1.XInt),
            nameof(Dummy1.XNullableString)
        }, propertyNames);
    }

    [Fact]
    public void IgnoreTargetProperty()
    {
        var mapper = new XMapper<Dummy1, Dummy2>(PropertyList.Target)
            .IgnoreTargetProperty(x => x.XNullableInt)
            .IgnoreTargetProperty(x => x.XString)
            .IgnoreTargetProperty(x => x.XEnum)
            .IgnoreTargetProperty(x => x.XNullableEnum);
        var propertyNames = GetPrivatePropertyInfos(mapper).OrderBy(x => x.Name).Select(x => x.Name);

        Assert.Equal(new[]
        {
            nameof(Dummy2.XInt),
            nameof(Dummy2.XNullableString),
            nameof(Dummy2.XNullableString2)
        }, propertyNames);
    }
}
