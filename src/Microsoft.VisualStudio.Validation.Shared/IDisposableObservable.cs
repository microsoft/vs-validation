/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

namespace Microsoft
{
    using System;

    /// <summary>
    /// A disposable object that also provides a safe way to query its disposed status.
    /// </summary>
    public interface IDisposableObservable : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether this instance has been disposed.
        /// </summary>
        bool IsDisposed { get; }
    }
}
