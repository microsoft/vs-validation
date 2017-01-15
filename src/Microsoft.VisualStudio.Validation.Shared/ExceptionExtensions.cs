/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

namespace Microsoft
{
    using System;

    /// <summary>
    /// Extension methods for exceptions.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Adds data to the Data member of <paramref name="exception"/> before returning the modified exception.
        /// </summary>
        /// <typeparam name="T">The type of exception being modified.</typeparam>
        /// <param name="exception">The exception to add data to.</param>
        /// <param name="key">The key to use for the added data.</param>
        /// <param name="values">The values to add with the given <paramref name="key"/>.</param>
        /// <returns>A reference to the same <paramref name="exception"/>.</returns>
        public static T AddData<T>(this T exception, string key, params object[] values)
            where T : Exception
        {
            Requires.NotNull(exception, nameof(exception));

            if (values?.Length > 0)
            {
                exception.Data.Add(key, values);
            }

            return exception;
        }
    }
}
