/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

namespace Microsoft
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
#if NET45
    using System.Runtime.Serialization;
#endif

    /// <content>
    /// Contains the inner exception thrown by Assumes.
    /// </content>
    public partial class Assumes
    {
        /// <summary>
        /// The exception that is thrown when an internal assumption failed.
        /// </summary>
#if NET45
        [Serializable]
#endif
        [SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic", Justification = "Internal exceptions should not be caught.")]
        [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "This is an internal exception type and we don't use the recommended ctors.")]
        private sealed class InternalErrorException : Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="InternalErrorException"/> class.
            /// </summary>
            [DebuggerStepThrough]
            public InternalErrorException(string message = null, bool showAssert = true)
                : base(message ?? Strings.InternalExceptionMessage)
            {
                this.ShowAssertDialog(showAssert);
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="InternalErrorException"/> class.
            /// </summary>
            [DebuggerStepThrough]
            public InternalErrorException(string message, Exception innerException, bool showAssert = true)
                : base(message ?? Strings.InternalExceptionMessage, innerException)
            {
                this.ShowAssertDialog(showAssert);
            }

#if NET45
            /// <summary>
            /// Initializes a new instance of the <see cref="InternalErrorException"/> class.
            /// </summary>
            [DebuggerStepThrough]
            private InternalErrorException(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
            }
#endif

            /// <summary>
            /// Show the assert if showAssert==true.
            /// </summary>
            /// <param name="showAssert">Whether to show the assert.</param>
            /// <remarks>
            /// The assertion dialog may yet be suppressed if
            /// ((DefaultTraceListener)System.Diagnostics.Trace.Listeners["Default"]).AssertUiEnabled == false
            /// </remarks>
            [DebuggerStepThrough]
            private void ShowAssertDialog(bool showAssert)
            {
                if (showAssert)
                {
                    // In debug builds, throw up a dialog.  This allows a dev to
                    // attach a debugger right at the point where the exception is
                    // thrown rather than at the point where the exception is caught.
                    string message = this.Message;
                    if (this.InnerException != null)
                    {
                        message += " " + this.InnerException;
                    }

                    Report.Fail(message);
                }
            }
        }
    }
}
