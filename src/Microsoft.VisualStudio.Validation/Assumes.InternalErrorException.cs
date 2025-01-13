// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft;

/// <content>
/// Contains the inner exception thrown by Assumes.
/// </content>
public partial class Assumes
{
    /// <summary>
    /// The exception that is thrown when an internal assumption failed.
    /// </summary>
    [Serializable]
    private sealed class InternalErrorException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InternalErrorException"/> class.
        /// </summary>
        [DebuggerStepThrough]
        public InternalErrorException(string? message = null)
            : base(message ?? Strings.InternalExceptionMessage)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InternalErrorException"/> class.
        /// </summary>
        [DebuggerStepThrough]
        public InternalErrorException(string? message, Exception? innerException)
            : base(message ?? Strings.InternalExceptionMessage, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InternalErrorException"/> class.
        /// </summary>
        [DebuggerStepThrough]
        [Obsolete]
        private InternalErrorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
