/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

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
    public static partial class Verify
    {
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
