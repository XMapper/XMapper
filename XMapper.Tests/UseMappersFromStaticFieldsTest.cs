using Xunit;

namespace XMapper.Tests;
public class UseMappersFromStaticFieldsTest
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

    public static XMapper<MemberA, MemberB> mXm = new XMapper<MemberA, MemberB>(PropertyList.Target);
    public static XMapper<DummyA, DummyB> mapper = new XMapper<DummyA, DummyB>(PropertyList.Target)
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

    [Theory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]

    public void Test(bool sourceNull, bool targetNull)
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
