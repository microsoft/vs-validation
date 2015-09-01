using System;
using Microsoft;
using Xunit;

public class RequiresTests
{
    [Fact]
    public void NotNull_ThrowsOnNull()
    {
        Assert.Throws<ArgumentNullException>(() => Requires.NotNull<object>((object)null, "foo"));
    }
}
