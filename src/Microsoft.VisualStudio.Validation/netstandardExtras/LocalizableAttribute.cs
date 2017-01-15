/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

#if NETSTANDARD1_0 || PROFILE259
namespace System.ComponentModel
{
    /// <summary>
    /// Marks whether a parameter (or other element) is meant to contain localizable text.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    internal sealed class LocalizableAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizableAttribute"/> class.
        /// </summary>
        public LocalizableAttribute(bool isLocalizable)
        {
        }
    }
}
#endif
