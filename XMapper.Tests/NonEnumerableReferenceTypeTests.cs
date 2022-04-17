using Xunit;

namespace XMapper.Tests;
public class NonEnumerableReferenceTypeMemberTests
{
    public class DummyA
    {
        public Member1? XMember1 { get; set; }
        public string? XNullableString { get; set; }
        public int? XNullableInt { get; set; }
    }

    public class DummyB
    {
        public Member2? XMember2 { get; set; }
        public string? XNullableString { get; set; }
    }

    public class Member1
    {
        public int? V { get; set; } = 1;
    }

    public class Member2
    {
        public int? V { get; set; } = 2;
    }

    [Theory]
    [InlineData(true, TestTarget.IsNull)]
    [InlineData(true, TestTarget.HasNullReference)]
    [InlineData(true, TestTarget.HasInstance)]
    [InlineData(false, TestTarget.IsNull)]
    [InlineData(false, TestTarget.HasNullReference)]
    [InlineData(false, TestTarget.HasInstance)]
    public void DifferentType(bool sourceHasNullReference, TestTarget testTarget)
    {
        var dA = new DummyA
        {
            XMember1 = sourceHasNullReference ? null : new Member1(),
            XNullableInt = 5,
            XNullableString = "Mapped",
        };

        var mXm = new XMapper<Member1, Member2>(PropertyList.Source);
        var mapper = new XMapper<DummyA, DummyB>(PropertyList.Target)
            .IgnoreTargetProperty(x => x.XMember2)
            .IncludeAction((source, target) =>
            {
                if (source.XMember1 == null)
                {
                    target.XMember2 = null;
                }
                else
                {
                    mXm.Map(source.XMember1, target.XMember2 ??= new());
                }
            });

        DummyB dB;
        switch (testTarget)
        {
            case TestTarget.IsNull:
                dB = mapper.Map(dA);
                break;
            case TestTarget.HasNullReference:
                dB = new DummyB { XMember2 = null };
                mapper.Map(dA, dB);
                break;
            case TestTarget.HasInstance:
            default:
                dB = new DummyB { XMember2 = new() };
                mapper.Map(dA, dB);
                break;
        }

        if (sourceHasNullReference)
        {
            Assert.Null(dB.XMember2);
        }
        else
        {
            Assert.Equal(dA.XMember1!.V, dB.XMember2!.V);
        }
        Assert.Equal(dA.XNullableString, dB.XNullableString);
    }


    public class DummyC
    {
        public Member1? XMember1 { get; set; }
        public string? XNullableString { get; set; }
        public int? XNullableInt { get; set; }
    }

    [Theory]
    [InlineData(true, TestTarget.IsNull)]
    [InlineData(true, TestTarget.HasNullReference)]
    [InlineData(true, TestTarget.HasInstance)]
    [InlineData(false, TestTarget.IsNull)]
    [InlineData(false, TestTarget.HasNullReference)]
    [InlineData(false, TestTarget.HasInstance)]
    public void SameType(bool sourceHasNullReference, TestTarget testTarget)
    {
        var dA = new DummyA
        {
            XMember1 = sourceHasNullReference ? null : new Member1 { V = 56 },
            XNullableInt = 5,
            XNullableString = "Mapped",
        };

        var mapper = new XMapper<DummyA, DummyC>(PropertyList.Target);

        DummyC dC;
        switch (testTarget)
        {
            case TestTarget.IsNull:
                dC = mapper.Map(dA);
                break;
            case TestTarget.HasNullReference:
                dC = new DummyC { XMember1 = null };
                mapper.Map(dA, dC);
                break;
            case TestTarget.HasInstance:
            default:
                dC = new DummyC { XMember1 = new() };
                mapper.Map(dA, dC);
                break;
        }

        if (sourceHasNullReference)
        {
            Assert.Null(dC.XMember1);
        }
        else
        {
            Assert.Equal(dA.XMember1, dC.XMember1);
        }
        Assert.Equal(dA.XNullableString, dC.XNullableString);
    }
    public enum TestTarget
    {
        IsNull,
        HasNullReference,
        HasInstance,
    }
}
