﻿using System;
using Xunit;

namespace XMapper.Tests.ErrorMessages;
public class MemberTypeMismatch
{
    public class DummyA
    {
        public string XString { get; set; } = "";
    }

    public class DummyB
    {
        public int XString { get; set; }
    }

    [Fact]
    public void NotNull()
    {
        var mapper = new XMapper<DummyA, DummyB>(PropertyList.Source);
        var ex = Assert.ThrowsAny<Exception>(() => mapper.Map(new()));
        Assert.Equal("'DummyA.XString' is of type 'System.String', but 'DummyB.XString' is of type 'System.Int32'.", ex.Message);
    }

    public class DummyG
    {
        public int? XInt { get; set; }
    }

    public class DummyH
    {
        public int XInt { get; set; } = 2;
    }

    [Fact]
    public void StrictNullCheck()
    {
        var mapper = new XMapper<DummyG, DummyH>(PropertyList.Source);
        var ex = Assert.ThrowsAny<Exception>(() => mapper.Map(new()));
        Assert.Equal("'DummyG.XInt' was null, but 'DummyH.XInt' is not nullable.", ex.Message);
    }
}
