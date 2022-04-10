using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace XMapper.Tests;
public class IncludeEnumerableMemberWithValueTypeElementsTests
{
    public class DummyA
    {
        public int XInt { get; set; } = 6;
        public int[]? XIntArray { get; set; }
    }

    public class DummyB
    {
        public int XInt { get; set; } = 7;
        public List<int>? XIntList { get; set; }
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
            XIntArray = sourceHasNullReference ? null : new[] { 1 }
        };


        var mapper = new XMapper<DummyA, DummyB>(PropertyList.Source)
            .IncludeAction((source, target) => target.XIntList = source.XIntArray?.ToList());

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
            Assert.Null(dB.XIntList);
        }
        else
        {
            var element = Assert.Single(dB.XIntList);
            Assert.Equal(1, element);
        }
        Assert.Equal(6, dB.XInt);
    }
}
