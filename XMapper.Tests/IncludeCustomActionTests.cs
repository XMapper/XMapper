using Xunit;

namespace XMapper.Tests;
public class IncludeCustomActionTests
{
    [Theory]
    [InlineData(null)]
    [InlineData(DummyEnum.None)]
    public void DependingOnCondition(DummyEnum? ne)
    {
        var dummy1 = new Dummy1 { XNullableEnum = DummyEnum.One, XNullableInt = 2, XString = "Map me!" };
        var dummy2 = new Dummy2 { XNullableEnum = ne, XNullableInt = 1 };

        var mapper = new XMapper<Dummy1, Dummy2>(PropertyList.Source)
            .IgnoreSourceProperty(x => x.XNullableEnum)
            .IncludeAction((source, target) =>
            {
                if (target.XNullableEnum == null)
                {
                    target.XNullableEnum = source.XNullableEnum;
                }
            });

        mapper.Map(dummy1, dummy2);

        Assert.Equal(ne == null ? dummy1.XNullableEnum : ne, dummy2.XNullableEnum);
        Assert.Equal(2, dummy2.XNullableInt);
        Assert.Equal(dummy1.XString, dummy2.XString);
    }
}
