#nullable enable

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft;
using Xunit;

public class RequiresTests
{
    [Fact]
    public void NotNull_ThrowsOnNull()
    {
        Assert.Throws<ArgumentNullException>(() => Requires.NotNull((object)null!, "foo"));
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
        Assert.Throws<ArgumentNullException>(() => Requires.NotNull((Task)null!, "foo"));
        Requires.NotNull((Task)Task.FromResult(0), "foo");
    }

    [Fact]
    public void NotNull_TaskOfT_ThrowsOnNull()
    {
        Assert.Throws<ArgumentNullException>(() => Requires.NotNull((Task<int>)null!, "foo"));
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
    public void Argument_Bool_String_String_Object()
    {
        Requires.Argument(true, "a", "b");
        Assert.Throws<ArgumentException>("a", () => Requires.Argument(false, "a", "b", "arg1"));
    }

    [Fact]
    public void Argument_Bool_String_String_Object_Object()
    {
        Requires.Argument(true, "a", "b");
        Assert.Throws<ArgumentException>("a", () => Requires.Argument(false, "a", "b", "arg1", "arg2"));
    }

    [Fact]
    public void Argument_Bool_String_String_ObjectArray()
    {
        Requires.Argument(true, "a", "b");
        Assert.Throws<ArgumentException>("a", () => Requires.Argument(false, "a", "b", "arg1", "arg2", "arg3"));
    }

    [Fact]
    public void Fail()
    {
        Assert.Throws<ArgumentException>(() => Requires.Fail("message"));
    }

    [Fact]
    public void Fail_ObjectArray()
    {
        Assert.Throws<ArgumentException>(() => Requires.Fail("message", "arg1"));
    }

    [Fact]
    public void Fail_Exception_ObjectArray()
    {
        ArgumentException ex = Assert.Throws<ArgumentException>(() => Requires.Fail(new InvalidOperationException(), "message", "arg1"));
        Assert.IsType<InvalidOperationException>(ex.InnerException);
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

    [Fact]
    public void NotNullAllowStructs()
    {
        Requires.NotNullAllowStructs(0, "paramName");
        Assert.Throws<ArgumentNullException>(() => Requires.NotNullAllowStructs((object?)null, "paramName"));
    }

    [Fact]
    public void NotNullOrEmpty()
    {
        Requires.NotNullOrEmpty("not empty", "param");
        Assert.Throws<ArgumentNullException>(() => Requires.NotNullOrEmpty(null!, "paramName"));
        Assert.Throws<ArgumentException>(() => Requires.NotNullOrEmpty(string.Empty, "paramName"));
        Assert.Throws<ArgumentException>(() => Requires.NotNullOrEmpty("\0", "paramName"));
        ArgumentException ex = Assert.Throws<ArgumentException>(() => Requires.NotNullOrEmpty(string.Empty, null));
        Assert.Null(ex.ParamName);
    }

    [Fact]
    public void NotNullOrWhitespace()
    {
        Requires.NotNullOrEmpty("not empty", "param");
        Assert.Throws<ArgumentNullException>(() => Requires.NotNullOrWhiteSpace(null!, "paramName"));
        Assert.Throws<ArgumentException>(() => Requires.NotNullOrWhiteSpace(string.Empty, "paramName"));
        Assert.Throws<ArgumentException>(() => Requires.NotNullOrWhiteSpace("\0", "paramName"));
        ArgumentException ex = Assert.Throws<ArgumentException>(() => Requires.NotNullOrWhiteSpace(" \t\n\r ", "paramName"));
        Assert.Equal("paramName", ex.ParamName);
    }

    [Fact]
    public void NotNullOrEmpty_Collection()
    {
        ICollection<string>? nullCollection = null;
        ICollection<string> emptyCollection = new string[0];
        ICollection<string> collection = new[] { "hi" };
        Requires.NotNullOrEmpty(collection, "param");
        Assert.Throws<ArgumentNullException>(() => Requires.NotNullOrEmpty(nullCollection!, "param"));
        Assert.Throws<ArgumentException>(() => Requires.NotNullOrEmpty(emptyCollection, "param"));
    }

    [Fact]
    public void NotNullEmptyOrNullElements()
    {
        ICollection<string>? nullCollection = null;
        ICollection<string> emptyCollection = new string[0];
        ICollection<string> collection = new[] { "hi" };
        ICollection<string> collectionWithNullElement = new[] { "hi", null!, "bye" };

        Requires.NotNullEmptyOrNullElements(collection, "param");
        Assert.Throws<ArgumentNullException>(() => Requires.NotNullEmptyOrNullElements(nullCollection!, "param"));
        Assert.Throws<ArgumentException>(() => Requires.NotNullEmptyOrNullElements(emptyCollection, "param"));
        Assert.Throws<ArgumentException>(() => Requires.NotNullEmptyOrNullElements(collectionWithNullElement, "param"));
    }

    [Fact]
    public void NullOrNotNullElements()
    {
        IEnumerable<string>? nullCollection = null;
        IEnumerable<string> emptyCollection = new string[0];
        IEnumerable<string> collection = new[] { "hi" };
        IEnumerable<string?> collectionWithNullElement = new[] { "hi", null, "bye" };

        Requires.NullOrNotNullElements(nullCollection!, "param");
        Requires.NullOrNotNullElements(emptyCollection, "param");
        Requires.NullOrNotNullElements(collection, "param");
        Assert.Throws<ArgumentException>(() => Requires.NullOrNotNullElements(collectionWithNullElement, "param"));
    }
}
