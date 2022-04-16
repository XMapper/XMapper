using System;
using Xunit;

namespace XMapper.Tests.ErrorMessages;
public class AssignNullToNotNullable
{
    public class DummyA
    {
        public int? XInt { get; set; }
    }

    public class DummyB
    {
        public int XInt { get; set; } = 2;
    }

    [Theory]
    [InlineData(PropertyList.Source)]
    [InlineData(PropertyList.Target)]
    public void Test(PropertyList propertyList)
    {
        var mapper = new XMapper<DummyA, DummyB>(propertyList);

        var ex = Assert.ThrowsAny<Exception>(() => mapper.Map(new DummyA()));

        Assert.Contains("'DummyA.XInt' was null, but 'DummyB.XInt' is not nullable.", ex.Message);
    }
}
