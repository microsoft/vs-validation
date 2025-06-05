// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;

/// <summary>
/// Suppresses the managed Assertion Failure dialog box, and continues
/// to log assertion failures to the debug output.
/// </summary>
/// <remarks>
/// Inspired by Matt Ellis' post at:
/// <see href="http://blogs.msdn.com/bclteam/archive/2007/07/19/customizing-the-behavior-of-system-diagnostics-debug-assert-matt-ellis.aspx"/>.
/// </remarks>
internal class AssertDialogSuppression : IDisposable
{
    /// <summary>
    /// Stores the original popup-ability of the assertion dialog.
    /// </summary>
    private bool? originalAssertUiSetting = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssertDialogSuppression"/> class,
    /// and immediately begins suppressing assertion dialog popups.
    /// </summary>
    public AssertDialogSuppression()
    {
#if NETCOREAPP
        Trace.Listeners.Clear();
#else
        // We disable the assertion dialog so it doesn't block tests, as we expect some tests to test failure cases.
        if (Trace.Listeners["Default"] is DefaultTraceListener assertDialogListener)
        {
            this.originalAssertUiSetting = assertDialogListener.AssertUiEnabled;
            assertDialogListener.AssertUiEnabled = false;
        }

        // Xunit.v3 v2 also adds a TraceListener that throws on failure, so remove that too.
        // See also https://github.com/xunit/xunit/issues/3317.
        // My mechanism for removing the listener is designed to work before and after that issue is resolved.
        if (Trace.Listeners.OfType<Xunit.Internal.TraceAssertOverrideListener>().FirstOrDefault() is { } listener)
        {
            Trace.Listeners.Remove(listener);
        }
#endif
    }

    /// <summary>
    /// Stops suppressing the assertion dialog and restores its popup-ability to whatever it was
    /// (either on or off) when this object was instantiated.
    /// </summary>
    public void Dispose()
    {
        if (this.originalAssertUiSetting.HasValue)
        {
            if (Trace.Listeners["Default"] is DefaultTraceListener assertDialogListener)
            {
                assertDialogListener.AssertUiEnabled = this.originalAssertUiSetting.Value;
            }
        }
    }
}
