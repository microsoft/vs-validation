/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

namespace System.Runtime
{
    /// <summary>
    /// A hint to ngen that it is preferrable that a method's implementation be shared
    /// across assembly boundaries in order to avoid a method call.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal sealed class TargetedPatchingOptOutAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TargetedPatchingOptOutAttribute"/> class.
        /// </summary>
        public TargetedPatchingOptOutAttribute(string reason)
        {
        }
    }
}