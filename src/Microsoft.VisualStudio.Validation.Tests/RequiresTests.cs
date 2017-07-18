using System;
using System.Threading.Tasks;
using Microsoft;
using Xunit;

public class RequiresTests
{
    [Fact]
    public void NotNull_ThrowsOnNull()
    {
        Assert.Throws<ArgumentNullException>(() => Requires.NotNull((object)null, "foo"));
        Requires.NotNull(new object(), "foo");
    }

    [Fact]
    public void NotNull_IntPtr_ThrowsOnZero()
    {
        Assert.Throws<ArgumentNullException>(() => Requires.NotNull(IntPtr.Zero, "foo"));
        Requires.NotNull(new IntPtr(5), "foo");
    }

    [Fact]
    public void NotNull_Task_ThrowsOnNull()
    {
        Assert.Throws<ArgumentNullException>(() => Requires.NotNull((Task)null, "foo"));
        Requires.NotNull((Task)Task.FromResult(0), "foo");
    }

    [Fact]
    public void NotNull_TaskOfT_ThrowsOnNull()
    {
        Assert.Throws<ArgumentNullException>(() => Requires.NotNull((Task<int>)null, "foo"));
        Requires.NotNull(Task.FromResult(0), "foo");
    }

    [Fact]
    public void Guid_ThrowsOnEmpty()
    {
        // Make sure no exception is thrown when a non empty guid is passed
        Requires.NotEmpty(Guid.NewGuid(), "foo");

        // Make sure an argument exception is thrown when an empty guid is passed in.
        Assert.Throws<ArgumentException>(() => Requires.NotEmpty(Guid.Empty, "foo"));
    }

    [Fact]
    public void Argument_Bool_String_String()
    {
        Requires.Argument(true, "a", "b");
        Assert.Throws<ArgumentException>("a", () => Requires.Argument(false, "a", "b"));
    }

    [Fact]
    public void Range_Bool_String_String()
    {
        Requires.Range(true, "a");
        Requires.Range(true, "a", "b");
        Assert.Throws<ArgumentOutOfRangeException>("a", () => Requires.Range(false, "a", "b"));
        Assert.Throws<ArgumentOutOfRangeException>("a", () => Requires.Range(false, "a"));
    }

    [Fact]
    public void FailRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>("a", () => Requires.FailRange("a"));
    }
}
