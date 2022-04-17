using Xunit;

namespace XMapper.Tests;
public class IncludeValueTypeViaCustomMappingTests
{
    public class DummyA
    {
        public string XString { get; set; } = "";
        public int? XNullableInt { get; set; }
        public DummyEnum? XNullableEnum { get; set; }
    }

    public class DummyB
    {
        public string XString { get; set; } = "";
        public int? XNullableInt { get; set; }
        public DummyEnum? XNullableEnum { get; set; }
    }

    public enum DummyEnum
    {
        None,
        One,
    }

    [Theory]
    [InlineData(null)]
    [InlineData(DummyEnum.None)]
    public void DependingOnCondition(DummyEnum? ne)
    {
        var dA = new DummyA { XNullableEnum = DummyEnum.One, XNullableInt = 2, XString = "Map me!" };
        var dB = new DummyB { XNullableEnum = ne, XNullableInt = 1 };

        var mapper = new XMapper<DummyA, DummyB>(PropertyList.Source)
            .IgnoreSourceProperty(x => x.XNullableEnum)
            .IncludeAction((source, target) =>
            {
                if (target.XNullableEnum == null)
                {
                    target.XNullableEnum = source.XNullableEnum;
                }
            });

        mapper.Map(dA, dB);

        Assert.Equal(ne == null ? dA.XNullableEnum : ne, dB.XNullableEnum);
        Assert.Equal(2, dB.XNullableInt);
        Assert.Equal(dA.XString, dB.XString);
    }
}
