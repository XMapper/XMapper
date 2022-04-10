using Xunit;

namespace XMapper.Tests;
public class IncludeValueTypeViaCustomMappingTests
{
    [Theory]
    [InlineData(null)]
    [InlineData(DummyEnum.None)]
    public void DependingOnCondition(DummyEnum? ne)
    {
        var d1 = new Dummy1 { XNullableEnum = DummyEnum.One, XNullableInt = 2, XString = "Map me!" };
        var d2 = new Dummy2 { XNullableEnum = ne, XNullableInt = 1 };

        var mapper = new XMapper<Dummy1, Dummy2>(PropertyList.Source)
            .IgnoreSourceProperty(x => x.XNullableEnum)
            .IncludeAction((source, target) =>
            {
                if (target.XNullableEnum == null)
                {
                    target.XNullableEnum = source.XNullableEnum;
                }
            });

        mapper.Map(d1, d2);

        Assert.Equal(ne == null ? d1.XNullableEnum : ne, d2.XNullableEnum);
        Assert.Equal(2, d2.XNullableInt);
        Assert.Equal(d1.XString, d2.XString);
    }
}
