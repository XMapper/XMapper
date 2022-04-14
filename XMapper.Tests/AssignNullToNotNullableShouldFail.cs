using System;
using Xunit;

namespace XMapper.Tests;
public class AssignNullToNotNullableShouldFail
{
    public class DummyG
    {
        public int? XInt { get; set; }
    }

    public class DummyH
    {
        public int XInt { get; set; } = 2;
    }

    [Theory]
    [InlineData(PropertyList.Source)]
    [InlineData(PropertyList.Target)]
    public void Test(PropertyList propertyList)
    {
        var mapper = new XMapper<DummyG, DummyH>(propertyList);

        var ex = Assert.ThrowsAny<Exception>(() => mapper.Map(new DummyG()));
        Assert.Contains("DummyG.XInt", ex.ToString());
        Assert.Contains("nullable", ex.ToString());
        Assert.Contains("DummyH.XInt", ex.ToString());
    }
}
