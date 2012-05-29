//-----------------------------------------------------------------------
// <copyright file="Report.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Common runtime checks that trace messages and invoke an assertion failure,
    /// but does *not* throw exceptions.
    /// </summary>
    public static class Report
    {
        /// <summary>
        /// Verifies that a value is not null, and reports an error about a missing MEF component otherwise.
        /// </summary>
        /// <typeparam name="T">The interface of the imported part.</typeparam>
        [DebuggerStepThrough]
        public static void IfNotPresent<T>(T part)
        {
            if (part == null)
            {
#if NET35
                Type coreType = typeof(T);
#else
                Type coreType = PrivateErrorHelpers.TrimGenericWrapper(typeof(T), typeof(Lazy<>));
#endif
                if (Environment.GetEnvironmentVariable("CPSUnitTest") != "true")
                {
                    Fail(Strings.ServiceMissing, coreType.FullName);
                }
            }
        }

        /// <summary>
        /// Reports an error if a condition evaluates to true.
        /// </summary>
        [DebuggerStepThrough]
        public static void If(bool condition, string message = null)
        {
            if (condition)
            {
                Fail(message);
            }
        }

        /// <summary>
        /// Reports an error if a condition does not evaluate to true.
        /// </summary>
        [DebuggerStepThrough]
        public static void IfNot(bool condition, string message = null)
        {
            if (!condition)
            {
                Fail(message);
            }
        }

        /// <summary>
        /// Reports an error if a condition does not evaluate to true.
        /// </summary>
        [DebuggerStepThrough]
        public static void IfNot(bool condition, string message, object arg1)
        {
            if (!condition)
            {
                Fail(PrivateErrorHelpers.Format(message, arg1));
            }
        }

        /// <summary>
        /// Reports an error if a condition does not evaluate to true.
        /// </summary>
        [DebuggerStepThrough]
        public static void IfNot(bool condition, string message, object arg1, object arg2)
        {
            if (!condition)
            {
                Fail(PrivateErrorHelpers.Format(message, arg1, arg2));
            }
        }

        /// <summary>
        /// Reports an error if a condition does not evaluate to true.
        /// </summary>
        [DebuggerStepThrough]
        public static void IfNot(bool condition, string message, params object[] args)
        {
            if (!condition)
            {
                Fail(PrivateErrorHelpers.Format(message, args));
            }
        }

        /// <summary>
        /// Reports a certain failure.
        /// </summary>
        [DebuggerStepThrough]
        public static void Fail(string message = null)
        {
            if (message == null)
            {
                message = "An public error occurred.";
            }

            Debug.WriteLine(message);
            Debug.Fail(message);
        }

        /// <summary>
        /// Reports a certain failure.
        /// </summary>
        [DebuggerStepThrough]
        public static void Fail(string message, params object[] args)
        {
            Fail(PrivateErrorHelpers.Format(message, args));
        }
    }
}
