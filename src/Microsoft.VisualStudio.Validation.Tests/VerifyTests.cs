using System;
using Microsoft;
using Xunit;

public class VerifyTests
{
    [Fact]
    public void Operation()
    {
        Verify.Operation(true, "Should not throw");
        Assert.Throws<InvalidOperationException>(() => Verify.Operation(false, "throw"));
    }
}
