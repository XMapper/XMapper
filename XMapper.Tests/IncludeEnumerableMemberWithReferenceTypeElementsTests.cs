using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace XMapper.Tests;
public class IncludeEnumerableMemberWithReferenceTypeElementsTests
{
    public class DummyA
    {
        public int XInt { get; set; } = 6;
        public Dummy1[]? XDummy1Array { get; set; }
    }

    public class DummyB
    {
        public int XInt { get; set; } = 7;
        public List<Dummy2>? XDummy2List { get; set; }
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, false)]
    [InlineData(false, true)]
    public void IncludeEnumerableMemberWithReferenceTypeElements(bool sourceHasNullReference, bool targetExists)
    {
        var dA = new DummyA
        {
            XDummy1Array = sourceHasNullReference ? null : new Dummy1[] { new Dummy1 { XInt = 1 } }
        };


        var d1Xd2 = new XMapper<Dummy1, Dummy2>(PropertyList.Source);
        var mapper = new XMapper<DummyA, DummyB>(PropertyList.Source)
            .IncludeAction((source, target) => target.XDummy2List = source.XDummy1Array?.Select(x => d1Xd2.Map(x)).ToList());

        DummyB dB;
        if (targetExists)
        {
            dB = new DummyB();
            mapper.Map(dA, dB);
        }
        else
        {
            dB = mapper.Map(dA);
        }

        if (sourceHasNullReference)
        {
            Assert.Null(dB.XDummy2List);
        }
        else
        {
            var d2 = Assert.Single(dB.XDummy2List);
            Assert.Equal(1, d2.XInt);
        }
        Assert.Equal(6, dB.XInt);
    }
}
