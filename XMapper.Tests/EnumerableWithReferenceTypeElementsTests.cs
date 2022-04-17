using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace XMapper.Tests;
public class EnumerableWithReferenceTypeElementsTests
{
    public class DummyA
    {
        public int XInt { get; set; } = 6;
        public MemberA[]? XMemberAArray { get; set; }
    }

    public class DummyB
    {
        public int XInt { get; set; } = 7;
        public List<MemberB>? XMemberBList { get; set; }
    }

    public class MemberA { public string XString { get; set; } = "A"; }
    public class MemberB { public string? XString { get; set; } = "B"; }

    [Theory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, false)]
    [InlineData(false, true)]
    public void ToDifferentEnumerableTypeWithDifferentElementType(bool sourceHasNullReference, bool targetExists)
    {
        var dA = new DummyA
        {
            XMemberAArray = sourceHasNullReference ? null : new MemberA[] { new MemberA() }
        };


        var mXm = new XMapper<MemberA, MemberB>(PropertyList.Source);
        var mapper = new XMapper<DummyA, DummyB>(PropertyList.Source)
            .IgnoreSourceProperty(x => x.XMemberAArray)
            .IncludeAction((source, target) => target.XMemberBList = source.XMemberAArray?.Select(x => mXm.Map(x)).ToList());

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
            Assert.Null(dB.XMemberBList);
        }
        else
        {
            var mB = Assert.Single(dB.XMemberBList);
            Assert.Equal("A", mB.XString);
        }
        Assert.Equal(6, dB.XInt);
    }

    public class DummyC
    {
        public MemberA[]? XMemberAArray { get; set; } = Array.Empty<MemberA>();
    }

    public class DummyD
    {
        public MemberA[]? XMemberAArray { get; set; } = Array.Empty<MemberA>();
    }


    [Theory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, false)]
    [InlineData(false, true)]
    public void ToSameType(bool sourceHasNullReference, bool targetExists)
    {
        var dA = new DummyC
        {
            XMemberAArray = sourceHasNullReference ? null : new MemberA[] { new MemberA { XString = "Map me!" } }
        };

        var mapper = new XMapper<DummyC, DummyD>(PropertyList.Source);

        DummyD dD;
        if (targetExists)
        {
            dD = new DummyD();
            mapper.Map(dA, dD);
        }
        else
        {
            dD = mapper.Map(dA);
        }

        if (sourceHasNullReference)
        {
            Assert.Null(dD.XMemberAArray);
        }
        else
        {
            var mB = Assert.Single(dD.XMemberAArray);
            Assert.Equal("Map me!", mB.XString);
        }
    }

    public class DummyE
    {
        public List<MemberA>? XMemberAList { get; set; } = new();
    }


    [Theory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, false)]
    [InlineData(false, true)]
    public void ToDifferentEnumerableType(bool sourceHasNullReference, bool targetExists)
    {
        var dA = new DummyC
        {
            XMemberAArray = sourceHasNullReference ? null : new MemberA[] { new MemberA { XString = "Map me!" } }
        };

        var mapper = new XMapper<DummyC, DummyE>(PropertyList.Source)
            .IgnoreSourceProperty(x => x.XMemberAArray)
            .IncludeAction((source, target) => target.XMemberAList = source.XMemberAArray?.ToList());

        DummyE dE;
        if (targetExists)
        {
            dE = new DummyE();
            mapper.Map(dA, dE);
        }
        else
        {
            dE = mapper.Map(dA);
        }

        if (sourceHasNullReference)
        {
            Assert.Null(dE.XMemberAList);
        }
        else
        {
            var mB = Assert.Single(dE.XMemberAList);
            Assert.Equal("Map me!", mB.XString);
        }
    }
}
