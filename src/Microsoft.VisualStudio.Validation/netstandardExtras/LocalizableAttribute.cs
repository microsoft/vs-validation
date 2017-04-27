/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

#if NETSTANDARD1_0 || PROFILE259
namespace System.ComponentModel
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Marks whether a parameter (or other element) is meant to contain localizable text.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    internal sealed class LocalizableAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizableAttribute"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "There is no need to consume the parameter. It's the mere presence of the parameter that lets analyzers of callers know what to do.")]
        public LocalizableAttribute(bool isLocalizable)
        {
        }
    }
}
#endif
