using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace XMapper.Tests;

public class InitialPropertyListTests
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
        public IEnumerable<int> XEnumerable { get; set; } = Enumerable.Empty<int>();
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

    private static List<PropertyInfo> GetPrivatePropertyInfos(object mapper)
    {
        return (List<PropertyInfo>)mapper.GetType().GetRuntimeFields().First(x => x.Name == "_propertyInfos").GetValue(mapper)!;
    }


    [Fact]
    public void SourceProperties_Dummy1()
    {
        var mapper = new XMapper<Dummy1, Dummy2>(PropertyList.Source);
        var propertyNames = GetPrivatePropertyInfos(mapper).OrderBy(x => x.Name).Select(x => x.Name);

        Assert.Equal(new[]
        {
            nameof(Dummy1.XEnum),
            nameof(Dummy1.XEnumerable),
            nameof(Dummy1.XInt),
            nameof(Dummy1.XNullableEnum),
            nameof(Dummy1.XNullableInt),
            nameof(Dummy1.XNullableString),
            nameof(Dummy1.XString),
            nameof(Dummy1.XType),
        }, propertyNames);
    }

    [Fact]
    public void SourceProperties_Dummy3()
    {
        var mapper = new XMapper<Dummy3, Dummy2>(PropertyList.Source);
        var propertyInfo = Assert.Single(GetPrivatePropertyInfos(mapper));
        Assert.Equal(nameof(Dummy3.XNullableString), propertyInfo.Name);
    }

    [Fact]
    public void TargetProperties_Dummy2()
    {
        var mapper = new XMapper<Dummy1, Dummy2>(PropertyList.Target);
        var propertyNames = GetPrivatePropertyInfos(mapper).OrderBy(x => x.Name).Select(x => x.Name);

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
        var mapper = new XMapper<Dummy1, Dummy3>(PropertyList.Target);
        var propertyInfo = Assert.Single(GetPrivatePropertyInfos(mapper));
        Assert.Equal(nameof(Dummy3.XNullableString), propertyInfo.Name);
    }
}
