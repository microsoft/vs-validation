using System;
using System.Runtime.InteropServices;
using Microsoft;
using Xunit;

public class VerifyTests
{
    [Fact]
    public void Operation()
    {
        Verify.Operation(true, "Should not throw");
        Verify.Operation(true, "Should not throw", "arg1");
        Verify.Operation(true, "Should not throw", "arg1", "arg2");
        Verify.Operation(true, "Should not throw", "arg1", "arg2", "arg3");

        Assert.Throws<InvalidOperationException>(() => Verify.Operation(false, "throw"));
        Assert.Throws<InvalidOperationException>(() => Verify.Operation(false, "throw", "arg1"));
        Assert.Throws<InvalidOperationException>(() => Verify.Operation(false, "throw", "arg1", "arg2"));
        Assert.Throws<InvalidOperationException>(() => Verify.Operation(false, "throw", "arg1", "arg2", "arg3"));
    }

    [Fact]
    public void OperationWithHelp()
    {
        Verify.OperationWithHelp(true, "message", "helpLink");
        try
        {
            Verify.OperationWithHelp(false, "message", "helpLink");
            Assert.True(false, "Expected exception not thrown");
        }
        catch (InvalidOperationException ex)
        {
            Assert.Equal("message", ex.Message);
            Assert.Equal("helpLink", ex.HelpLink);
        }
    }

    [Fact]
    public void NotDisposed()
    {
        Verify.NotDisposed(true, "message");
        ObjectDisposedException actualException = Assert.Throws<ObjectDisposedException>(() => Verify.NotDisposed(false, "message"));
        Assert.Equal(string.Empty, actualException.ObjectName);
        Assert.Equal("message", actualException.Message);

        Assert.Throws<ObjectDisposedException>(() => Verify.NotDisposed(false, null));
        Assert.Throws<ObjectDisposedException>(() => Verify.NotDisposed(false, (object)null));

        try
        {
            Verify.NotDisposed(false, "hi", "message");
            Assert.False(true, "Expected exception not thrown.");
        }
        catch (ObjectDisposedException ex)
        {
            string expectedObjectName = typeof(string).FullName;
            Assert.Equal(expectedObjectName, ex.ObjectName);
            Assert.Equal(new ObjectDisposedException(expectedObjectName, "message").Message, ex.Message);
        }

        try
        {
            Verify.NotDisposed(false, new object());
            Assert.False(true, "Expected exception not thrown.");
        }
        catch (ObjectDisposedException ex)
        {
            Assert.Equal(typeof(object).FullName, ex.ObjectName);
        }
    }

    [Fact]
    public void NotDisposed_Observable()
    {
        var observable = new Disposable();
        Verify.NotDisposed(observable);
        observable.Dispose();
        Assert.Throws<ObjectDisposedException>(() => Verify.NotDisposed(observable));
        Assert.Throws<ObjectDisposedException>(() => Verify.NotDisposed(observable, "message"));
    }

    [Fact]
    public void FailOperation()
    {
        Assert.Throws<InvalidOperationException>(() => Verify.FailOperation("message", "arg1"));
    }

    [Fact]
    public void HResult()
    {
        const int E_INALIDARG = unchecked((int)0x80070057);
        const int E_FAIL = unchecked((int)0x80004005);
        Verify.HResult(0);
        Assert.Throws<ArgumentException>(() => Verify.HResult(E_INALIDARG));
        Assert.Throws<COMException>(() => Verify.HResult(E_FAIL));
        Assert.Throws<ArgumentException>(() => Verify.HResult(E_INALIDARG, ignorePreviousComCalls: true));
        Assert.Throws<COMException>(() => Verify.HResult(E_FAIL, ignorePreviousComCalls: true));
    }

    private class Disposable : IDisposableObservable
    {
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            this.IsDisposed = true;
        }
    }
}
