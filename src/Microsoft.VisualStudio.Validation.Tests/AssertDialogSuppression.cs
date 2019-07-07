using System;
using System.Diagnostics;

/// <summary>
/// Suppresses the managed Assertion Failure dialog box, and continues
/// to log assertion failures to the debug output.
/// </summary>
/// <remarks>
/// Inspired by Matt Ellis' post at:
/// http://blogs.msdn.com/bclteam/archive/2007/07/19/customizing-the-behavior-of-system-diagnostics-debug-assert-matt-ellis.aspx
/// </remarks>
internal class AssertDialogSuppression : IDisposable
{
    /// <summary>
    /// Stores the original popup-ability of the assertion dialog.
    /// </summary>
    private bool? originalAssertUiSetting;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssertDialogSuppression"/> class,
    /// and immediately begins suppressing assertion dialog popups.
    /// </summary>
    public AssertDialogSuppression()
    {
        // We disable the assertion dialog so it doesn't block tests, as we expect some tests to test failure cases.
        if (Trace.Listeners["Default"] is DefaultTraceListener assertDialogListener)
        {
            this.originalAssertUiSetting = assertDialogListener.AssertUiEnabled;
            assertDialogListener.AssertUiEnabled = false;
        }
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
