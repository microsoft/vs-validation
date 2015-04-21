/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

namespace Microsoft
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Indicates to Code Analysis that a method validates a particular parameter.
    /// </summary>
#if !NET35
    [ExcludeFromCodeCoverage]
#endif
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class ValidatedNotNullAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatedNotNullAttribute"/> class.
        /// </summary>
        public ValidatedNotNullAttribute()
        {
        }
    }
}
