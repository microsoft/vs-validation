//-----------------------------------------------------------------------
// <copyright file="Verify.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Common runtime checks that throw exceptions upon failure.
    /// </summary>
    public static class Verify
    {
        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/> if a condition is false.
        /// </summary>
        [DebuggerStepThrough]
        public static void Operation(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/> if a condition is false.
        /// </summary>
        [DebuggerStepThrough]
        public static void Operation(bool condition, string unformattedMessage, object arg1)
        {
            if (!condition)
            {
                throw new InvalidOperationException(PrivateErrorHelpers.Format(unformattedMessage, arg1));
            }
        }

        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/> if a condition is false.
        /// </summary>
        [DebuggerStepThrough]
        public static void Operation(bool condition, string unformattedMessage, object arg1, object arg2)
        {
            if (!condition)
            {
                throw new InvalidOperationException(PrivateErrorHelpers.Format(unformattedMessage, arg1, arg2));
            }
        }

        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/> if a condition is false.
        /// </summary>
        [DebuggerStepThrough]
        public static void Operation(bool condition, string unformattedMessage, params object[] args)
        {
            if (!condition)
            {
                throw new InvalidOperationException(PrivateErrorHelpers.Format(unformattedMessage, args));
            }
        }

        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/> if a condition is false.
        /// </summary>
        [DebuggerStepThrough]
        public static void OperationWithHelp(bool condition, string message, string helpLink)
        {
            if (!condition)
            {
                var ex = new InvalidOperationException(message);
                ex.HelpLink = helpLink;
                throw ex;
            }
        }

        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <returns>
        /// Nothing.  This method always throws.
        /// The signature claims to return an exception to allow callers to throw this method
        /// to satisfy C# execution path constraints.
        /// </returns>
        [DebuggerStepThrough]
        public static Exception FailOperation(string message, params object[] args)
        {
            throw new InvalidOperationException(PrivateErrorHelpers.Format(message, args));
        }

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> if an object is disposed.
        /// </summary>
        [DebuggerStepThrough]
        public static void NotDisposed(IDisposableObservable disposedValue, string message = null)
        {
            Requires.NotNull(disposedValue, nameof(disposedValue));

            if (disposedValue.IsDisposed)
            {
                string objectName = disposedValue != null ? disposedValue.GetType().FullName : string.Empty;
                if (message != null)
                {
                    throw new ObjectDisposedException(objectName, message);
                }
                else
                {
                    throw new ObjectDisposedException(objectName);
                }
            }
        }

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> if a condition is false.
        /// </summary>
        [DebuggerStepThrough]
        public static void NotDisposed(bool condition, object disposedValue, string message = null)
        {
            if (!condition)
            {
                string objectName = disposedValue != null ? disposedValue.GetType().FullName : string.Empty;
                if (message != null)
                {
                    throw new ObjectDisposedException(objectName, message);
                }
                else
                {
                    throw new ObjectDisposedException(objectName);
                }
            }
        }

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> if a condition is false.
        /// </summary>
        [DebuggerStepThrough]
        public static void NotDisposed(bool condition, string message)
        {
            if (!condition)
            {
                throw new ObjectDisposedException(message);
            }
        }

        /// <summary>
        /// Throws an exception if the given value is negative.
        /// </summary>
        /// <param name="hresult">The HRESULT corresponding to the desired exception.</param>
        /// <param name="ignorePreviousComCalls">If true, prevents <c>ThrowExceptionForHR</c> from returning an exception from a previous COM call and instead always use the HRESULT specified.</param>
        /// <remarks>
        /// No exception is thrown for S_FALSE.
        /// </remarks>
        [DebuggerStepThrough]
        public static void HResult(int hresult, bool ignorePreviousComCalls = false)
        {
            if (hresult < 0)
            {
                if (ignorePreviousComCalls)
                {
                    Marshal.ThrowExceptionForHR(hresult, new IntPtr(-1));
                }
                else
                {
                    Marshal.ThrowExceptionForHR(hresult);
                }
            }
        }
    }
}
