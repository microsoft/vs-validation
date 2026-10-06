// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft;
using Moq;
using TUnit.Core;
using Xunit;

public partial class AssumesTests : IDisposable
{
    private const string TestMessage = "Some test message.";
    private readonly AssertDialogSuppression suppressAssertUi = new();
    private readonly OverrideCulture overrideCulture = new(CultureInfo.InvariantCulture);

    public void Dispose()
    {
        this.suppressAssertUi.Dispose();
        this.overrideCulture.Dispose();
    }

    [Test]
    public void True()
    {
        Assumes.True(true);
        Assert.ThrowsAny<Exception>(() => Assumes.True(false, TestMessage));
        Assert.ThrowsAny<Exception>(() => Assumes.True(false, TestMessage, "arg1"));
        Assert.ThrowsAny<Exception>(() => Assumes.True(false, TestMessage, "arg1", "arg2"));
    }

    [Test]
    public void False()
    {
        Assumes.False(false);
        Assert.ThrowsAny<Exception>(() => Assumes.False(true, TestMessage));
        Assert.ThrowsAny<Exception>(() => Assumes.False(true, TestMessage, "arg1"));
        Assert.ThrowsAny<Exception>(() => Assumes.False(true, TestMessage, "arg1", "arg2"));
    }

    [Test]
    public void True_InterpolatedString()
    {
        int formatCount = 0;
        string FormattingMethod()
        {
            formatCount++;
            return "generated string";
        }

        Assumes.True(true, $"Some {FormattingMethod()} method.");
        Assert.Equal(0, formatCount);

        Exception ex = Assert.ThrowsAny<Exception>(() => Assumes.True(false, $"Some {FormattingMethod()} method."));
        Assert.Equal(1, formatCount);
        Assert.StartsWith("Some generated string method.", ex.Message);
    }

    [Test]
    public void False_InterpolatedString()
    {
        int formatCount = 0;
        string FormattingMethod()
        {
            formatCount++;
            return "generated string";
        }

        Assumes.False(false, $"Some {FormattingMethod()} method.");
        Assert.Equal(0, formatCount);

        Exception ex = Assert.ThrowsAny<Exception>(() => Assumes.False(true, $"Some {FormattingMethod()} method."));
        Assert.Equal(1, formatCount);
        Assert.StartsWith("Some generated string method.", ex.Message);
    }

    [Test]
    public void Fail()
    {
        Assert.ThrowsAny<Exception>(() => Assumes.Fail("some message", new InvalidOperationException()));
    }

    [Test]
    public void NotNull()
    {
        Exception ex = Assert.ThrowsAny<Exception>(() => Assumes.NotNull((string?)null));

        Assert.Equal("Unexpected null value of type 'String'.", ex.Message);

        Assumes.NotNull("success");
    }

    [Test]
    public void NotNull_NullableStruct()
    {
        Exception ex = Assert.ThrowsAny<Exception>(() => Assumes.NotNull((int?)null));

        Assert.Equal("Unexpected null value of type 'Int32'.", ex.Message);

        Assumes.NotNull((int?)5);
    }

    [Test]
    public void Null()
    {
        Exception ex = Assert.ThrowsAny<Exception>(() => Assumes.Null("not null"));

        Assert.Equal("Unexpected non-null value of type 'String'.", ex.Message);

        Assumes.Null((object?)null);
    }

    [Test]
    public void Null_NullableStruct()
    {
        Exception ex = Assert.ThrowsAny<Exception>(() => Assumes.Null((int?)5));

        Assert.Equal("Unexpected non-null value of type 'Int32'.", ex.Message);

        Assumes.Null((int?)null);
    }

    [Test]
    public void NotNullOrEmpty()
    {
        ICollection<string> collection = new string[] { "foo" };

        Assert.ThrowsAny<Exception>(() => Assumes.NotNullOrEmpty(null));
        Assert.ThrowsAny<Exception>(() => Assumes.NotNullOrEmpty(string.Empty));
        Assumes.NotNullOrEmpty("success");

        Assert.ThrowsAny<Exception>(() => Assumes.NotNullOrEmpty((ICollection<string>?)null));
        Assert.ThrowsAny<Exception>(() => Assumes.NotNullOrEmpty(collection.Take(0).ToList()));
        Assumes.NotNullOrEmpty(collection);

        Assert.ThrowsAny<Exception>(() => Assumes.NotNullOrEmpty((IEnumerable<string>?)null));
        Assert.ThrowsAny<Exception>(() => Assumes.NotNullOrEmpty(collection.Take(0)));
        Assumes.NotNullOrEmpty(collection.Take(1));
    }

    [Test]
    public void NotNullOrEmpty_EnumerableOfT_AlsoImplementsICollectionOfT()
    {
        // Mock type that implements both IEnumerable<T> and ICollection<T>
        var collection = new Mock<ICollection<string>>(MockBehavior.Strict);
        Mock<IEnumerable<string>> enumerable = collection.As<IEnumerable<string>>();

        enumerable.Setup(m => m.GetEnumerator()).Throws(new Exception("Should not call GetEnumerator."));

        collection.SetupGet(m => m.Count).Returns(0);
        Assert.ThrowsAny<Exception>(() => Assumes.NotNullOrEmpty(enumerable.Object));

        collection.SetupGet(m => m.Count).Returns(1);
        Assumes.NotNullOrEmpty(enumerable.Object);
    }

    [Test]
    public void NotNullOrEmpty_EnumerableOfT_AlsoImplementsIReadOnlyCollectionOfT()
    {
        // Mock type that implements both IEnumerable<T> and IReadOnlyCollection<T>
        var collection = new Mock<IReadOnlyCollection<string>>(MockBehavior.Strict);
        Mock<IEnumerable<string>> enumerable = collection.As<IEnumerable<string>>();

        enumerable.Setup(m => m.GetEnumerator()).Throws(new Exception("Should not call GetEnumerator."));

        collection.SetupGet(m => m.Count).Returns(0);
        Assert.ThrowsAny<Exception>(() => Assumes.NotNullOrEmpty(enumerable.Object));

        collection.SetupGet(m => m.Count).Returns(1);
        Assumes.NotNullOrEmpty(enumerable.Object);
    }

    [Test]
    public void Is()
    {
        Assert.ThrowsAny<Exception>(() => Assumes.Is<string>(null));
        Assert.ThrowsAny<Exception>(() => Assumes.Is<string>(45));
        Assert.ThrowsAny<Exception>(() => Assumes.Is<string>(new object()));
        Assumes.Is<string>("hi");
    }

    [Test]
    public void NotReachable()
    {
        Assert.ThrowsAny<Exception>(Assumes.NotReachable);
    }

    [Test]
    public void NotReachableOfT()
    {
        Assert.ThrowsAny<Exception>(() => Assumes.NotReachable<int>());
        Assert.ThrowsAny<Exception>(() => Assumes.NotReachable<object>());
    }

    [Test]
    public void Present()
    {
        IServiceProvider? someService = null;
        Assert.ThrowsAny<Exception>(() => Assumes.Present(someService));
        Assumes.Present("hi");
    }

#if NETFRAMEWORK
    [Test]
    public void InternalErrorException_IsSerializable()
    {
        try
        {
            Assumes.False(true);
        }
        catch (Exception ex)
        {
            var formatter = new BinaryFormatter();
            using var ms = new MemoryStream();
            formatter.Serialize(ms, ex);
            ms.Position = 0;
            var ex2 = (Exception)formatter.Deserialize(ms);
            Assert.IsType(ex.GetType(), ex2);
            Assert.Equal(ex.Message, ex2.Message);
        }
    }
#endif
}
