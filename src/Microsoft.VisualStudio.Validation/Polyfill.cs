// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if !NET5_0_OR_GREATER

#pragma warning disable SA1649 // File name should match first type name
#pragma warning disable SA1600 // Elements should be documented

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    internal sealed class CallerArgumentExpressionAttribute : Attribute
    {
        internal CallerArgumentExpressionAttribute(string parameterName)
        {
        }
    }
}

#endif
