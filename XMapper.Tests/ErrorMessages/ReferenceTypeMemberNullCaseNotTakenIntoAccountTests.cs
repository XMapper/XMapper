using System;
using Xunit;

namespace XMapper.Tests.ErrorMessages;
public class ReferenceTypeMemberNullCaseNotTakenIntoAccountTests
{
    public class DummyA
    {
        public MemberA? TheMember { get; set; }
    }

    public class DummyB
    {
        public MemberB? TheMember { get; set; }
    }

    public class MemberA { }
    public class MemberB { }


    [Fact]
    public void NonEnumerable_Invalid_1()
    {
        var mXm = new XMapper<MemberA, MemberB>(PropertyList.Target);
        var mapper = new XMapper<DummyA, DummyB>(PropertyList.Target)
            .IgnoreTargetProperty(x => x.TheMember)
            .IncludeAction((source, target) => mXm.Map(source.TheMember!, target.TheMember!));

        var source = new DummyA { TheMember = new() };
        var target = new DummyB { TheMember = null };

        var exception = Assert.ThrowsAny<Exception>(() => mapper.Map(source, target));

        Assert.Contains("Argument 'target' in 'XMapper<MemberA, MemberB>.Map(...)' should not be null.", exception.Message);
    }

    [Fact]
    public void NonEnumerable_Invalid_2()
    {
        var mXm = new XMapper<MemberA, MemberB>(PropertyList.Target);
        var mapper = new XMapper<DummyA, DummyB>(PropertyList.Target)
            .IgnoreTargetProperty(x => x.TheMember)
            .IncludeAction((source, target) => mXm.Map(source.TheMember!, target.TheMember ??= new()));

        var source = new DummyA { TheMember = null };
        var target = new DummyB { TheMember = null };

        var exception = Assert.ThrowsAny<Exception>(() => mapper.Map(source, target));

        Assert.Contains("Argument 'source' in 'XMapper<MemberA, MemberB>.Map(...)' should not be null.", exception.Message);
    }

    [Fact]
    public void NonEnumerable_Invalid_3()
    {
        var mXm = new XMapper<MemberA, MemberB>(PropertyList.Target);
        var mapper = new XMapper<DummyA, DummyB>(PropertyList.Target)
            .IgnoreTargetProperty(x => x.TheMember)
            .IncludeAction((source, target) =>
            {
                if (source.TheMember == null)
                {
                    target.TheMember = null;
                }
                else
                {
                    mXm.Map(source.TheMember, target.TheMember!);
                }
            });

        var source = new DummyA { TheMember = new() };
        var target = new DummyB { TheMember = null };

        var exception = Assert.ThrowsAny<Exception>(() => mapper.Map(source, target));

        Assert.Contains("Argument 'target' in 'XMapper<MemberA, MemberB>.Map(...)' should not be null.", exception.Message);
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]

    public void NonEnumerable_Valid(bool sourceNull, bool targetNull)
    {
        var mXm = new XMapper<MemberA, MemberB>(PropertyList.Target);
        var mapper = new XMapper<DummyA, DummyB>(PropertyList.Target)
            .IgnoreTargetProperty(x => x.TheMember)
            .IncludeAction((source, target) =>
            {
                if (source.TheMember == null)
                {
                    target.TheMember = null;
                }
                else
                {
                    mXm.Map(source.TheMember, target.TheMember ??= new());
                }
            });

        var dA = new DummyA { TheMember = sourceNull ? null : new() };
        var dB = new DummyB { TheMember = targetNull ? null : new() };

        Does.NotThrow(() => mapper.Map(dA, dB));
    }
}
