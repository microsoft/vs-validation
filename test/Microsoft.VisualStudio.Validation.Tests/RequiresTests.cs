// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft;
using Xunit;
using Xunit.Abstractions;

public class RequiresTests
{
    private readonly ITestOutputHelper logger;

    public RequiresTests(ITestOutputHelper logger)
    {
        this.logger = logger;
    }

    private enum BigEnum : long
    {
        First,
    }

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
    public void NotDefault()
    {
        Requires.NotDefault(-1, "foo");
        Assert.Throws<ArgumentException>(() => Requires.NotDefault(default(int), "foo"));

        Requires.NotDefault('a', "foo");
        Assert.Throws<ArgumentException>(() => Requires.NotDefault(default(char), "foo"));
        Assert.Throws<ArgumentException>(() => Requires.NotDefault('\0', "foo"));
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
        System.Collections.IEnumerable? nullCollection = null;
        System.Collections.IEnumerable emptyCollection = Array.Empty<string>();
        System.Collections.IEnumerable collection = new[] { "hi" };
        Requires.NotNullOrEmpty(collection, "param");
        Assert.Throws<ArgumentNullException>(() => Requires.NotNullOrEmpty(nullCollection!, "param"));
        Assert.Throws<ArgumentException>(() => Requires.NotNullOrEmpty(emptyCollection, "param"));
    }

    [Fact]
    public void NotNullOrEmpty_CollectionOfT()
    {
        IEnumerable<string>? nullCollection = null;
        IEnumerable<string> emptyCollection = Array.Empty<string>();
        IEnumerable<string> collection = new[] { "hi" };
        Requires.NotNullOrEmpty(collection, "param");
        Assert.Throws<ArgumentNullException>(() => Requires.NotNullOrEmpty(nullCollection!, "param"));
        Assert.Throws<ArgumentException>(() => Requires.NotNullOrEmpty(emptyCollection, "param"));
    }

    [Fact]
    public void NotNullOrEmpty_CollectionOfT_Struct()
    {
        IEnumerable<int>? nullCollection = null;
        IEnumerable<int> emptyCollection = Array.Empty<int>();
        IEnumerable<int> collection = new[] { 5 };
        Requires.NotNullOrEmpty(collection, "param");
        Assert.Throws<ArgumentNullException>(() => Requires.NotNullOrEmpty(nullCollection!, "param"));
        Assert.Throws<ArgumentException>(() => Requires.NotNullOrEmpty(emptyCollection, "param"));
    }

    [Fact]
    public void NotNullEmptyOrNullElements()
    {
        ICollection<string>? nullCollection = null;
        ICollection<string> emptyCollection = Array.Empty<string>();
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
        IEnumerable<string> emptyCollection = Array.Empty<string>();
        IEnumerable<string> collection = new[] { "hi" };
        IEnumerable<string?> collectionWithNullElement = new[] { "hi", null, "bye" };

        Requires.NullOrNotNullElements(nullCollection!, "param");
        Requires.NullOrNotNullElements(emptyCollection, "param");
        Requires.NullOrNotNullElements(collection, "param");
        Assert.Throws<ArgumentException>(() => Requires.NullOrNotNullElements(collectionWithNullElement, "param"));
    }

    [Fact]
    public void Defined()
    {
        Requires.Defined(ConsoleColor.Black, "parameterName");
        InvalidEnumArgumentException ex = Assert.Throws<InvalidEnumArgumentException>("parameterName", () => Requires.Defined((ConsoleColor)88, "parameterName"));
        this.logger.WriteLine(ex.Message);
    }

    [Fact]
    public void Defined_Int64Enum()
    {
        Requires.Defined(BigEnum.First, "parameterName");
        InvalidEnumArgumentException ex = Assert.Throws<InvalidEnumArgumentException>(() => Requires.Defined((BigEnum)0x100000000, "parameterName"));
        this.logger.WriteLine(ex.Message);
    }

    [Fact]
    public void ValidElements()
    {
        Assert.Throws<ArgumentNullException>("values", () => Requires.ValidElements((IEnumerable<string>)null!, x => !string.IsNullOrWhiteSpace(x), "param", "test"));
        Assert.Throws<ArgumentNullException>("predicate", () => Requires.ValidElements(new[] { -1 }, null!, "param", "Must be greater than 0."));
        Assert.Throws<ArgumentException>("param", () => Requires.ValidElements(new[] { -1 }, v => v > 0, "param", "Must be greater than 0."));
        Assert.Throws<ArgumentException>("param", () => Requires.ValidElements(new[] { "a", string.Empty, "b", null }, x => !string.IsNullOrWhiteSpace(x), "param", "test"));
        Assert.ThrowsAny<Exception>(() => Requires.ValidElements(new[] { "a", "b", "c", "d" }, x => throw new InvalidOperationException(), "param", "test"));
        Requires.ValidElements(new[] { "a", "b", "c", "d" }, x => !string.IsNullOrWhiteSpace(x), "param", "test");
        Requires.ValidElements(new[] { 1, 2 }, v => v > 0, "param", "Must be greater than 0.");

        Assert.Throws<ArgumentNullException>("values", () => Requires.ValidElements((IEnumerable<string>)null!, x => !string.IsNullOrWhiteSpace(x), "param", "test message: {0}", "param"));
        Assert.Throws<ArgumentNullException>("predicate", () => Requires.ValidElements(new[] { -1 }, null!, "param", "{0} must be greater than 0.", "param"));
        Assert.Contains("param must be greater than 0.", Assert.Throws<ArgumentException>("param", () => Requires.ValidElements(new[] { -1 }, v => v > 0, "param", "{0} must be greater than 0.", "param")).Message, StringComparison.InvariantCultureIgnoreCase);
        Assert.Contains("test message: param", Assert.Throws<ArgumentException>("param", () => Requires.ValidElements(new[] { "a", string.Empty, "b", null }, x => !string.IsNullOrWhiteSpace(x), "param", "test message: {0}", "param")).Message, StringComparison.InvariantCultureIgnoreCase);
        Assert.ThrowsAny<Exception>(() => Requires.ValidElements(new[] { "a", "b", "c", "d" }, x => throw new InvalidOperationException(), "param", "test message: {0}", "param"));
        Requires.ValidElements(new[] { "a", "b", "c", "d" }, x => !string.IsNullOrWhiteSpace(x), "param", "test message: {0}", "param");
        Requires.ValidElements(new[] { 1, 2 }, v => v > 0, "param", "{0} must be greater than 0.", "param");
    }
}
