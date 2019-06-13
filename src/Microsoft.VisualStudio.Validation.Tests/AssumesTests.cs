using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
#if !NETCOREAPP1_0
using System.Runtime.Serialization.Formatters.Binary;
#endif
using Microsoft;
using Xunit;

public partial class AssumesTests : IDisposable
{
    private const string TestMessage = "Some test message.";
    private AssertDialogSuppression suppressAssertUi = new AssertDialogSuppression();

    public void Dispose()
    {
        this.suppressAssertUi.Dispose();
    }

    [Fact]
    public void True()
    {
        Assumes.True(true);
        Assert.ThrowsAny<Exception>(() => Assumes.True(false, TestMessage));
        Assert.ThrowsAny<Exception>(() => Assumes.True(false, TestMessage, "arg1"));
        Assert.ThrowsAny<Exception>(() => Assumes.True(false, TestMessage, "arg1", "arg2"));
    }

    [Fact]
    public void False()
    {
        Assumes.False(false);
        Assert.ThrowsAny<Exception>(() => Assumes.False(true, TestMessage));
        Assert.ThrowsAny<Exception>(() => Assumes.False(true, TestMessage, "arg1"));
        Assert.ThrowsAny<Exception>(() => Assumes.False(true, TestMessage, "arg1", "arg2"));
    }

    [Fact]
    public void Fail()
    {
        Assert.ThrowsAny<Exception>(() => Assumes.Fail("some message", new InvalidOperationException()));
    }

    [Fact]
    public void NotNull()
    {
        Assert.ThrowsAny<Exception>(() => Assumes.NotNull((object)null));
        Assumes.NotNull("success");
    }

    [Fact]
    public void Null()
    {
        Assert.ThrowsAny<Exception>(() => Assumes.Null("not null"));
        Assumes.Null((object)null);
    }

    [Fact]
    public void NotNullOrEmpty()
    {
        ICollection<string> collection = new string[] { "foo" };

        Assert.ThrowsAny<Exception>(() => Assumes.NotNullOrEmpty(null));
        Assert.ThrowsAny<Exception>(() => Assumes.NotNullOrEmpty(string.Empty));
        Assumes.NotNullOrEmpty("success");

        Assert.ThrowsAny<Exception>(() => Assumes.NotNullOrEmpty((ICollection<string>)null));
        Assert.ThrowsAny<Exception>(() => Assumes.NotNullOrEmpty(collection.Take(0).ToList()));
        Assumes.NotNullOrEmpty(collection);

        Assert.ThrowsAny<Exception>(() => Assumes.NotNullOrEmpty((IEnumerable<string>)null));
        Assert.ThrowsAny<Exception>(() => Assumes.NotNullOrEmpty(collection.Take(0)));
        Assumes.NotNullOrEmpty(collection.Take(1));
    }

    [Fact]
    public void Is()
    {
        Assert.ThrowsAny<Exception>(() => Assumes.Is<string>(null));
        Assert.ThrowsAny<Exception>(() => Assumes.Is<string>(45));
        Assert.ThrowsAny<Exception>(() => Assumes.Is<string>(new object()));
        Assumes.Is<string>("hi");
    }

    [Fact]
    public void NotReachable()
    {
        Assert.ThrowsAny<Exception>(Assumes.NotReachable);
    }

    [Fact]
    public void Present()
    {
        IServiceProvider someService = null;
        Assert.ThrowsAny<Exception>(() => Assumes.Present(someService));
        Assumes.Present("hi");
    }

#if !NETCOREAPP1_0
    [Fact]
    public void InternalErrorException_IsSerializable()
    {
        try
        {
            Assumes.False(true);
        }
        catch (Exception ex)
        {
            var formatter = new BinaryFormatter();
            var ms = new MemoryStream();
            formatter.Serialize(ms, ex);
            ms.Position = 0;
            var ex2 = (Exception)formatter.Deserialize(ms);
            Assert.IsType(ex.GetType(), ex2);
            Assert.Equal(ex.Message, ex2.Message);
        }
    }
#endif
}
