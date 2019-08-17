/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

#if NETSTANDARD2_0

namespace System.Diagnostics.CodeAnalysis
{
    /// <summary>
    /// Specifies that the method will not return if the associated Boolean parameter is passed the specified value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    internal class DoesNotReturnIfAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoesNotReturnIfAttribute"/> class.
        /// </summary>
        /// <param name="parameterValue">
        /// The condition parameter value. Code after the method will be considered unreachable by diagnostics if the argument to
        /// the associated parameter matches this value.
        /// </param>
        public DoesNotReturnIfAttribute(bool parameterValue) => this.ParameterValue = parameterValue;

        /// <summary>Gets the condition parameter value.</summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:Property summary documentation should match accessors", Justification = "Comment taken as-is from corefx.")]
        public bool ParameterValue { get; }
    }
}

#endif
