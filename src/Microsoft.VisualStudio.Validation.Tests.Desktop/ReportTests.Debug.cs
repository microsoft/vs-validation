// Ensure the tests defined in this file always emulate a client compiled for Debug
#define DEBUG

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft;
using Moq;
using Xunit;

/// <summary>
/// Verify that the message propagates to the trace listeners if
/// the test project compiles with DEBUG (and the supplied condition is as appropriate).
/// </summary>
public class ReportDebugTests : IDisposable
{
    private const string FailureMessage = "failure";
    private const string DefaultFailureMessage = "A recoverable error has been detected.";

    public ReportDebugTests()
    {
        var defaultListener = Trace.Listeners.OfType<DefaultTraceListener>().FirstOrDefault();
        if (defaultListener != null)
        {
            defaultListener.AssertUiEnabled = false;
        }
    }

    public void Dispose()
    {
        var defaultListener = Trace.Listeners.OfType<DefaultTraceListener>().FirstOrDefault();
        if (defaultListener != null)
        {
            defaultListener.AssertUiEnabled = true;
        }
    }

    [Fact]
    public void If()
    {
        using (var listener = Listen())
        {
            Report.If(false, FailureMessage);
            listener.Value.Setup(l => l.WriteLine(FailureMessage)).Verifiable();
            listener.Value.Setup(l => l.Fail(FailureMessage)).Verifiable();
            Report.If(true, FailureMessage);
        }
    }

    [Fact]
    public void IfNot()
    {
        using (var listener = Listen())
        {
            Report.IfNot(true, FailureMessage);
            listener.Value.Setup(l => l.WriteLine(FailureMessage)).Verifiable();
            listener.Value.Setup(l => l.Fail(FailureMessage)).Verifiable();
            Report.IfNot(false, FailureMessage);
        }
    }

    [Fact]
    public void IfNot_Format1Arg()
    {
        using (var listener = Listen())
        {
            Report.IfNot(true, "a{0}c", "b");
            listener.Value.Setup(l => l.WriteLine("abc")).Verifiable();
            listener.Value.Setup(l => l.Fail("abc")).Verifiable();
            Report.IfNot(false, "a{0}c", "b");
        }
    }

    [Fact]
    public void IfNot_Format2Arg()
    {
        using (var listener = Listen())
        {
            Report.IfNot(true, "a{0}{1}d", "b", "c");
            listener.Value.Setup(l => l.WriteLine("abcd")).Verifiable();
            listener.Value.Setup(l => l.Fail("abcd")).Verifiable();
            Report.IfNot(false, "a{0}{1}d", "b", "c");
        }
    }

    [Fact]
    public void IfNot_FormatNArg()
    {
        using (var listener = Listen())
        {
            Report.IfNot(true, "a{0}{1}{2}e", "b", "c", "d");
            listener.Value.Setup(l => l.WriteLine("abcde")).Verifiable();
            listener.Value.Setup(l => l.Fail("abcde")).Verifiable();
            Report.IfNot(false, "a{0}{1}{2}e", "b", "c", "d");
        }
    }

    [Fact]
    public void IfNotPresent()
    {
        using (var listener = Listen())
        {
            var possiblyPresent = "not missing";
            var missingTypeName = possiblyPresent.GetType().FullName;
            Report.IfNotPresent(possiblyPresent);
            listener.Value.Setup(l => l.WriteLine(It.Is<string>(v => v.Contains(missingTypeName)))).Verifiable();
            listener.Value.Setup(l => l.Fail(It.Is<string>(v => v.Contains(missingTypeName)))).Verifiable();
            possiblyPresent = null;
            Report.IfNotPresent(possiblyPresent);
        }
    }

    [Fact]
    public void Fail()
    {
        using (var listener = Listen())
        {
            listener.Value.Setup(l => l.WriteLine(FailureMessage)).Verifiable();
            listener.Value.Setup(l => l.Fail(FailureMessage)).Verifiable();
            Report.Fail(FailureMessage);
        }
    }

    [Fact]
    public void Fail_DefaultMessage()
    {
        using (var listener = Listen())
        {
            listener.Value.Setup(l => l.WriteLine(DefaultFailureMessage)).Verifiable();
            listener.Value.Setup(l => l.Fail(DefaultFailureMessage)).Verifiable();
            Report.Fail();
        }
    }

    private static DisposableValue<Mock<TraceListener>> Listen()
    {
        var mockListener = new Mock<TraceListener>(MockBehavior.Strict);
        Trace.Listeners.Add(mockListener.Object);
        return new DisposableValue<Mock<TraceListener>>(
            mockListener,
            () =>
            {
                Trace.Listeners.Remove(mockListener.Object);
                mockListener.Verify();
            });
    }
}
