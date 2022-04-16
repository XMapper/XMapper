using System;
using Xunit;

namespace XMapper.Tests.ErrorMessages;
public class MemberNameMismatch
{
    public class DummyA
    {
        public string XString1 { get; set; } = "";
        public string XStringA2 { get; set; } = "";
    }

    public class DummyB
    {
        public string XString1 { get; set; } = "";
        public string XStringB2 { get; set; } = "";
    }

    [Fact]
    public void PropertyList_Source()
    {
        var mapper = new XMapper<DummyA, DummyB>(PropertyList.Source);
        var ex = Assert.Throws<Exception>(() => mapper.Map(new()));
        Assert.Contains("Property 'XStringA2' was not found on target 'DummyB'.", ex.Message);
    }

    [Fact]
    public void PropertyList_Target()
    {
        var mapper = new XMapper<DummyA, DummyB>(PropertyList.Target);
        var ex = Assert.Throws<Exception>(() => mapper.Map(new()));
        Assert.Contains("Property 'XStringB2' was not found on source 'DummyA'.", ex.Message);
    }
}
