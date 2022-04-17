using System;
using Xunit;

namespace XMapper.Tests.ErrorMessages;
public class NullableReferenceTypeToNotNullableReferenceType
{
    public class DummyA
    {
        public MemberA? TheMember { get; set; }
    }

    public class DummyB
    {
        public MemberB TheMember { get; set; } = new();
    }

    public class MemberA { }
    public class MemberB { }

    [Theory]
    [InlineData(PropertyList.Source)]
    [InlineData(PropertyList.Target)]
    public void Test(PropertyList propertyList)
    {
        var mXm = new XMapper<MemberA, MemberB>(propertyList);
        var mapper = new XMapper<DummyA, DummyB>(propertyList)
            .IncludeAction((source, target) =>
            {
                if (source.TheMember == null)
                {
                    target.TheMember = null!;
                }
                else
                {
                    mXm.Map(source.TheMember, target.TheMember ??= new());
                }
            });
        var ex = Assert.ThrowsAny<Exception>(() => mapper.Map(new()));
        Assert.Contains("'DummyA.TheMember' was null, but 'DummyB.TheMember' is not nullable.", ex.Message);
    }
}
