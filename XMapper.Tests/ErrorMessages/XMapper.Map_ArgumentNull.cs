using System;
using Xunit;

namespace XMapper.Tests.ErrorMessages;
public class XMapper_Map_ArgumentNull
{
    public class DummyA
    {
        public int XInt { get; set; } = 1;
    }

    public class DummyB
    {
        public int XInt { get; set; } = 2;
    }

    [Theory]
    [InlineData(PropertyList.Source)]
    [InlineData(PropertyList.Target)]
    public void Test1(PropertyList propertyList)
    {
        var mapper = new XMapper<DummyA, DummyB>(propertyList);

        var ex = Assert.ThrowsAny<Exception>(() => mapper.Map(null!));
        Assert.Contains("Argument 'source' in 'XMapper<DummyA, DummyB>.Map(...)' should not be null.", ex.Message);
    }

    [Theory]
    [InlineData(PropertyList.Source)]
    [InlineData(PropertyList.Target)]
    public void Test2(PropertyList propertyList)
    {
        var mapper = new XMapper<DummyA, DummyB>(propertyList);

        var sourceA = new DummyA();
        var ex = Assert.ThrowsAny<Exception>(() => mapper.Map(sourceA, null!));
        Assert.Contains("Argument 'target' in 'XMapper<DummyA, DummyB>.Map(...)' should not be null.", ex.Message);
    }

    [Theory]
    [InlineData(PropertyList.Source)]
    [InlineData(PropertyList.Target)]
    public void Test3(PropertyList propertyList)
    {
        var mapper = new XMapper<DummyA, DummyB>(propertyList);

        var target = new DummyB();
        var ex = Assert.ThrowsAny<Exception>(() => mapper.Map(null!, target));
        Assert.Contains("Argument 'source' in 'XMapper<DummyA, DummyB>.Map(...)' should not be null.", ex.Message);
    }
}
