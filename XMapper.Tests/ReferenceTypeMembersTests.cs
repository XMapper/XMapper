using Xunit;

namespace XMapper.Tests;
public class ReferenceTypeMembersTests
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

    [Fact]
    public void MapToReferenceTypeMember()
    {
        var dummy4 = new Dummy4
        {
            XDummy1 = new Dummy1
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

        var memberMapper = new XMapper<Dummy1, Dummy2>(UsePropertyListOf.Source);
        var mapper = new XMapper<Dummy4, Dummy5>(UsePropertyListOf.Target);

        var dummy5 = mapper.Map(dummy4);
        memberMapper.Map(dummy4.XDummy1, dummy5.XDummy2 ??= new Dummy2());

        Assert.Equal(dummy4.XDummy1.XEnum, dummy5.XDummy2.XEnum);
        Assert.Equal(dummy4.XDummy1.XInt, dummy5.XDummy2.XInt);
        Assert.Equal(dummy4.XDummy1.XNullableString, dummy5.XDummy2.XNullableString);
        Assert.Equal(dummy4.XDummy1.XNullableInt, dummy5.XDummy2.XNullableInt);
        Assert.Equal(dummy4.XDummy1.XString, dummy5.XDummy2.XString);
        Assert.Equal(dummy4.XNullableString, dummy5.XNullableString);
    }

    [Fact]
    public void RegisterPropertyMapper()
    {
        var dummy4 = new Dummy4
        {
            XDummy1 = new Dummy1
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

        var mapper = new XMapper<Dummy4, Dummy5>(UsePropertyListOf.Target)
            .RegisterPropertyMapper(s => s.XDummy1, t => t.XDummy2, new XMapper<Dummy1?, Dummy2?>(UsePropertyListOf.Source));

        var dummy5 = mapper.Map(dummy4);

        Assert.Equal(dummy4.XDummy1.XEnum, dummy5.XDummy2!.XEnum);
        Assert.Equal(dummy4.XDummy1.XInt, dummy5.XDummy2.XInt);
        Assert.Equal(dummy4.XDummy1.XNullableString, dummy5.XDummy2.XNullableString);
        Assert.Equal(dummy4.XDummy1.XNullableInt, dummy5.XDummy2.XNullableInt);
        Assert.Equal(dummy4.XDummy1.XString, dummy5.XDummy2.XString);
        Assert.Equal(dummy4.XNullableString, dummy5.XNullableString);
    }
}
