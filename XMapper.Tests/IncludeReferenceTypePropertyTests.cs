using Xunit;

namespace XMapper.Tests;
public class IncludeReferenceTypePropertyTests
{
    public class Dummy4
    {
        public Dummy1? XDummy1 { get; set; }
        public string? XNullableString { get; set; }
        public int? XNullableInt { get; set; }

    }

    public class Dummy5
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
    public void IncludeReferenceTypeProperty(bool sourceHasNullReference, TestTarget testTarget)
    {
        var dummy4 = new Dummy4
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

        var mapper = new XMapper<Dummy4, Dummy5>(PropertyList.Target)
            .IncludeReferenceTypeProperty(s => s.XDummy1, t => t.XDummy2, new XMapper<Dummy1?, Dummy2?>(PropertyList.Source));

        Dummy5 dummy5;
        switch (testTarget)
        {
            case TestTarget.IsNull:
                dummy5 = mapper.Map(dummy4);
                break;
            case TestTarget.HasNullReference:
                dummy5 = new Dummy5 { XDummy2 = null };
                mapper.Map(dummy4, dummy5);
                break;
            case TestTarget.HasInstance:
            default:
                dummy5 = new Dummy5 { XDummy2 = new() };
                mapper.Map(dummy4, dummy5);
                break;
        }

        if (sourceHasNullReference)
        {
            Assert.Null(dummy5.XDummy2);
        }
        else
        {
            Assert.Equal(dummy4.XDummy1!.XEnum, dummy5.XDummy2!.XEnum);
            Assert.Equal(dummy4.XDummy1.XInt, dummy5.XDummy2.XInt);
            Assert.Equal(dummy4.XDummy1.XNullableString, dummy5.XDummy2.XNullableString);
            Assert.Equal(dummy4.XDummy1.XNullableInt, dummy5.XDummy2.XNullableInt);
            Assert.Equal(dummy4.XDummy1.XString, dummy5.XDummy2.XString);
        }
        Assert.Equal(dummy4.XNullableString, dummy5.XNullableString);
    }
    public enum TestTarget
    {
        IsNull,
        HasNullReference,
        HasInstance,
    }
}
