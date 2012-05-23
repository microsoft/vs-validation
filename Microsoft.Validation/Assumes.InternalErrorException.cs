//-----------------------------------------------------------------------
// <copyright file="Assumes.InternalErrorException.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.Validation
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Runtime.Serialization;

    /// <content>
    /// Contains the inner exception thrown by Assumes.
    /// </content>
    internal partial class Assumes
    {
        /// <summary>
        /// The exception that is thrown when an internal assumption failed.
        /// </summary>
        [Serializable]
        [SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic", Justification = "Internal exceptions should not be caught.")]
        private class InternalErrorException : Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="InternalErrorException"/> class.
            /// </summary>
            public InternalErrorException(string message = null, bool showAssert = true)
                : base(message ?? Strings.InternalExceptionMessage)
            {
                ShowAssertDialog(showAssert);
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="InternalErrorException"/> class.
            /// </summary>
            public InternalErrorException(string message, Exception innerException, bool showAssert = true)
                : base(message ?? Strings.InternalExceptionMessage, innerException)
            {
                ShowAssertDialog(showAssert);
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="InternalErrorException"/> class.
            /// </summary>
            protected InternalErrorException(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
            }

            /// <summary>
            /// Show the assert if showAssert==true.
            /// </summary>
            /// <param name="showAssert">Whether to show the assert.</param>
            /// <remarks>
            /// The assertion dialog may yet be suppressed if 
            /// ((DefaultTraceListener)System.Diagnostics.Trace.Listeners["Default"]).AssertUiEnabled == false
            /// </remarks>
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
