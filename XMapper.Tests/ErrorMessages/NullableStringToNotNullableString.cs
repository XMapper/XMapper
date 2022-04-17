using System;
using Xunit;

namespace XMapper.Tests.ErrorMessages;
public class NullableStringToNotNullableString
{
    public class DummyA
    {
        public string? Value { get; set; }
    }

    public class DummyB
    {
        public string Value { get; set; } = "";
    }

    [Theory]
    [InlineData(PropertyList.Source)]
    [InlineData(PropertyList.Target)]
    public void Test(PropertyList propertyList)
    {
        var mapper = new XMapper<DummyA, DummyB>(propertyList);
        var ex = Assert.ThrowsAny<Exception>(() => mapper.Map(new()));
        Assert.Contains("'DummyA.Value' was null, but 'DummyB.Value' is not nullable.", ex.Message);
    }
}
