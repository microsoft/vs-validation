// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// Ensure the tests defined in this file always emulate a client compiled for Release
#undef DEBUG

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft;
using Moq;
using Xunit;

/// <summary>
/// Verify that the message does NOT propagate to the trace listeners when
/// the test project compiles without DEBUG.
/// </summary>
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name must match first type name", Justification = "By design")]
public class ReportReleaseTests : IDisposable
{
    private const string FailureMessage = "failure";

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
            Report.If(true, FailureMessage);
        }
    }

    [Fact]
    public void IfNot()
    {
        using (DisposableValue<Mock<TraceListener>> listener = Listen())
        {
            Report.IfNot(true, FailureMessage);
            Report.IfNot(false, FailureMessage);
        }
    }

    [Fact]
    public void IfNot_Format1Arg()
    {
        using (DisposableValue<Mock<TraceListener>> listener = Listen())
        {
            Report.IfNot(true, "a{0}c", "b");
            Report.IfNot(false, "a{0}c", "b");
        }
    }

    [Fact]
    public void IfNot_Format2Arg()
    {
        using (DisposableValue<Mock<TraceListener>> listener = Listen())
        {
            Report.IfNot(true, "a{0}{1}d", "b", "c");
            Report.IfNot(false, "a{0}{1}d", "b", "c");
        }
    }

    [Fact]
    public void IfNot_FormatNArg()
    {
        using (DisposableValue<Mock<TraceListener>> listener = Listen())
        {
            Report.IfNot(true, "a{0}{1}{2}e", "b", "c", "d");
            Report.IfNot(false, "a{0}{1}{2}e", "b", "c", "d");
        }
    }

    [Fact]
    public void IfNotPresent()
    {
        using (DisposableValue<Mock<TraceListener>> listener = Listen())
        {
            string? possiblyPresent = "not missing";
            var missingTypeName = possiblyPresent.GetType().FullName;
            Report.IfNotPresent(possiblyPresent);
            possiblyPresent = null;
            Report.IfNotPresent(possiblyPresent);
        }
    }

    [Fact]
    public void Fail()
    {
        using (DisposableValue<Mock<TraceListener>> listener = Listen())
        {
            Report.Fail(FailureMessage);
        }
    }

    [Fact]
    public void Fail_DefaultMessage()
    {
        using (DisposableValue<Mock<TraceListener>> listener = Listen())
        {
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
