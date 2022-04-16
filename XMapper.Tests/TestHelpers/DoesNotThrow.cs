using System;

namespace XMapper.Tests;

public static class Does
{
    public static void NotThrow(Action action)
    {
        action();
    }
}
