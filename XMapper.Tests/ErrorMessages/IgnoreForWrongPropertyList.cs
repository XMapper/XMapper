using System;
using Xunit;

namespace XMapper.Tests.ErrorMessages;
public class IgnoreForWrongPropertyList
{
    public class DummyA
    {
        public string XStringA { get; set; } = "";
    }

    public class DummyB
    {
        public string XStringA { get; set; } = "";
    }

    [Fact]
    public void IgnoreSourceProperty_while_PropertyList_is_Target()
    {
        var exception = Assert.Throws<Exception>(() =>
            new XMapper<DummyA, DummyB>(PropertyList.Target)
                .IgnoreSourceProperty(x => x.XStringA));
        Assert.Contains("Use 'IgnoreTargetProperty' if PropertyList is Target.", exception.Message);
    }

    [Fact]
    public void IgnoreTargetProperty_while_PropertyList_is_Target()
    {
        var exception = Assert.Throws<Exception>(() =>
            new XMapper<DummyA, DummyB>(PropertyList.Source)
                .IgnoreTargetProperty(x => x.XStringA));
        Assert.Contains("Use 'IgnoreSourceProperty' if PropertyList is Source.", exception.Message);
    }
}
