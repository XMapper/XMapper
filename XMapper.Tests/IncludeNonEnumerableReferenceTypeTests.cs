using Xunit;

namespace XMapper.Tests;
public class IncludeNonEnumerableReferenceTypeMemberTests
{
    public class DummyA
    {
        public Dummy1? XDummy1 { get; set; }
        public string? XNullableString { get; set; }
        public int? XNullableInt { get; set; }
    }

    public class DummyB
    {
        public Dummy2? XDummy2 { get; set; }
        public string? XNullableString { get; set; }
    }

    [Theory]
    [InlineData(true, TestTarget.IsNull)]
    [InlineData(true, TestTarget.HasNullReference)]
    [InlineData(true, TestTarget.HasInstance)]
    [InlineData(false, TestTarget.IsNull)]
    [InlineData(false, TestTarget.HasNullReference)]
    [InlineData(false, TestTarget.HasInstance)]
    public void NonEnumerableReferenceTypeMember(bool sourceHasNullReference, TestTarget testTarget)
    {
        var dA = new DummyA
        {
            XDummy1 = sourceHasNullReference ? null : new Dummy1
            {
                XEnum = DummyEnum.One,
                XInt = 1,
                XNullableEnum = DummyEnum.One,
                XNullableInt = null,
                XNullableString = "Not null anymore",
                XString = "Never null",
            },
            XNullableInt = 5,
            XNullableString = "Mapped",
        };

        var d1Xd2 = new XMapper<Dummy1, Dummy2>(PropertyList.Source);
        var mapper = new XMapper<DummyA, DummyB>(PropertyList.Target)
            .IncludeAction((source, target) =>
            {
                if (source.XDummy1 == null)
                {
                    target.XDummy2 = null;
                }
                else
                {
                    d1Xd2.Map(source.XDummy1, target.XDummy2 ??= new());
                }
            });

        DummyB dB;
        switch (testTarget)
        {
            case TestTarget.IsNull:
                dB = mapper.Map(dA);
                break;
            case TestTarget.HasNullReference:
                dB = new DummyB { XDummy2 = null };
                mapper.Map(dA, dB);
                break;
            case TestTarget.HasInstance:
            default:
                dB = new DummyB { XDummy2 = new() };
                mapper.Map(dA, dB);
                break;
        }

        if (sourceHasNullReference)
        {
            Assert.Null(dB.XDummy2);
        }
        else
        {
            Assert.Equal(dA.XDummy1!.XEnum, dB.XDummy2!.XEnum);
            Assert.Equal(dA.XDummy1.XInt, dB.XDummy2.XInt);
            Assert.Equal(dA.XDummy1.XNullableString, dB.XDummy2.XNullableString);
            Assert.Equal(dA.XDummy1.XNullableInt, dB.XDummy2.XNullableInt);
            Assert.Equal(dA.XDummy1.XString, dB.XDummy2.XString);
        }
        Assert.Equal(dA.XNullableString, dB.XNullableString);
    }
    public enum TestTarget
    {
        IsNull,
        HasNullReference,
        HasInstance,
    }
}
