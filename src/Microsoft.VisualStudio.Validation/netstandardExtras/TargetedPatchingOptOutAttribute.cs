/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

#if NETSTANDARD1_0 || PROFILE259
namespace System.Runtime
{
    using System.Diagnostics.CodeAnalysis;

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
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "There is no need to consume the parameter. It's the mere presence of the parameter that lets analyzers of callers know what to do.")]
        public TargetedPatchingOptOutAttribute(string reason)
        {
        }
    }
}
#endif
