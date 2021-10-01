// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// Ensure the tests defined in this file always emulate a client compiled for Debug
#define DEBUG

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft;
using Moq;
using Xunit;

/// <summary>
/// Verify that the message propagates to the trace listeners if
/// the test project compiles with DEBUG (and the supplied condition is as appropriate).
/// </summary>
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name must match first type name", Justification = "By design")]
public class ReportDebugTests : IDisposable
{
    private const string FailureMessage = "failure";
    private const string DefaultFailureMessage = "A recoverable error has been detected.";

    private AssertDialogSuppression suppressAssertUi = new AssertDialogSuppression();

    public void Dispose()
    {
        this.suppressAssertUi.Dispose();
    }

    [Fact]
    public void If()
    {
        using (DisposableValue<Mock<TraceListener>> listener = Listen())
        {
            Report.If(false, FailureMessage);
            listener.Value.Setup(l => l.WriteLine(FailureMessage)).Verifiable();
#if NETCOREAPP
            listener.Value.Setup(l => l.Fail(FailureMessage, string.Empty)).Verifiable();
#else
            listener.Value.Setup(l => l.Fail(FailureMessage)).Verifiable();
#endif
            Report.If(true, FailureMessage);
        }
    }

    [Fact]
    public void IfNot()
    {
        using (DisposableValue<Mock<TraceListener>> listener = Listen())
        {
            Report.IfNot(true, FailureMessage);
            listener.Value.Setup(l => l.WriteLine(FailureMessage)).Verifiable();
#if NETCOREAPP
            listener.Value.Setup(l => l.Fail(FailureMessage, string.Empty)).Verifiable();
#else
            listener.Value.Setup(l => l.Fail(FailureMessage)).Verifiable();
#endif
            Report.IfNot(false, FailureMessage);
        }
    }

    [Fact]
    public void IfNot_Format1Arg()
    {
        using (DisposableValue<Mock<TraceListener>> listener = Listen())
        {
            Report.IfNot(true, "a{0}c", "b");
            listener.Value.Setup(l => l.WriteLine("abc")).Verifiable();
#if NETCOREAPP
            listener.Value.Setup(l => l.Fail("abc", string.Empty)).Verifiable();
#else
            listener.Value.Setup(l => l.Fail("abc")).Verifiable();
#endif
            Report.IfNot(false, "a{0}c", "b");
        }
    }

    [Fact]
    public void IfNot_Format2Arg()
    {
        using (DisposableValue<Mock<TraceListener>> listener = Listen())
        {
            Report.IfNot(true, "a{0}{1}d", "b", "c");
            listener.Value.Setup(l => l.WriteLine("abcd")).Verifiable();
#if NETCOREAPP
            listener.Value.Setup(l => l.Fail("abcd", string.Empty)).Verifiable();
#else
            listener.Value.Setup(l => l.Fail("abcd")).Verifiable();
#endif
            Report.IfNot(false, "a{0}{1}d", "b", "c");
        }
    }

    [Fact]
    public void IfNot_FormatNArg()
    {
        using (DisposableValue<Mock<TraceListener>> listener = Listen())
        {
            Report.IfNot(true, "a{0}{1}{2}e", "b", "c", "d");
            listener.Value.Setup(l => l.WriteLine("abcde")).Verifiable();
#if NETCOREAPP
            listener.Value.Setup(l => l.Fail("abcde", string.Empty)).Verifiable();
#else
            listener.Value.Setup(l => l.Fail("abcde")).Verifiable();
#endif
            Report.IfNot(false, "a{0}{1}{2}e", "b", "c", "d");
        }
    }

    [Fact]
    public void IfNotPresent()
    {
        using (DisposableValue<Mock<TraceListener>> listener = Listen())
        {
            string? possiblyPresent = "not missing";
            string missingTypeName = possiblyPresent.GetType().FullName!;
            Report.IfNotPresent(possiblyPresent);
            listener.Value.Setup(l => l.WriteLine(It.Is<string>(v => v.Contains(missingTypeName)))).Verifiable();
#if NETCOREAPP
            listener.Value.Setup(l => l.Fail(It.Is<string>(v => v.Contains(missingTypeName)), string.Empty)).Verifiable();
#else
            listener.Value.Setup(l => l.Fail(It.Is<string>(v => v.Contains(missingTypeName)))).Verifiable();
#endif
            possiblyPresent = null;
            Report.IfNotPresent(possiblyPresent);
        }
    }

    [Fact]
    public void Fail()
    {
        using (DisposableValue<Mock<TraceListener>> listener = Listen())
        {
            listener.Value.Setup(l => l.WriteLine(FailureMessage)).Verifiable();
#if NETCOREAPP
            listener.Value.Setup(l => l.Fail(FailureMessage, string.Empty)).Verifiable();
#else
            listener.Value.Setup(l => l.Fail(FailureMessage)).Verifiable();
#endif
            Report.Fail(FailureMessage);
        }
    }

    [Fact]
    public void Fail_DefaultMessage()
    {
        using (DisposableValue<Mock<TraceListener>> listener = Listen())
        {
            listener.Value.Setup(l => l.WriteLine(DefaultFailureMessage)).Verifiable();
#if NETCOREAPP
            listener.Value.Setup(l => l.Fail(DefaultFailureMessage, string.Empty)).Verifiable();
#else
            listener.Value.Setup(l => l.Fail(DefaultFailureMessage)).Verifiable();
#endif
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
