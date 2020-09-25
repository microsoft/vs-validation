// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft
{
    using System;
    using System.Globalization;
    using System.Reflection;

    /// <summary>
    /// Common utility methods used by the various error detection and reporting classes.
    /// </summary>
    internal static class PrivateErrorHelpers
    {
        /// <summary>
        /// Trims away a given surrounding type, returning just the generic type argument,
        /// if the given type is in fact a generic type with just one type argument and
        /// the generic type matches a given wrapper type.  Otherwise, it returns the original type.
        /// </summary>
        /// <param name="type">The type to trim, or return unmodified.</param>
        /// <param name="wrapper">The SomeType&lt;&gt; generic type definition to trim away from <paramref name="type"/> if it is present.</param>
        /// <returns><paramref name="type"/>, if it is not a generic type instance of <paramref name="wrapper"/>; otherwise the type argument.</returns>
        internal static Type TrimGenericWrapper(Type type, Type? wrapper)
        {
            Type[] typeArgs;
            if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == wrapper && (typeArgs = type.GenericTypeArguments).Length == 1)
            {
                return typeArgs[0];
            }
            else
            {
                return type;
            }
        }

        /// <summary>
        /// Helper method that formats string arguments.
        /// </summary>
        /// <returns>The formatted string.</returns>
        internal static string Format(string format, params object?[] arguments)
        {
            return string.Format(CultureInfo.CurrentCulture, format, arguments);
        }
    }
}
