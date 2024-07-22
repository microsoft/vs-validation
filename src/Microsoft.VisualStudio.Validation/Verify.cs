// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft;

/// <summary>
/// Common runtime checks that throw exceptions upon failure.
/// </summary>
public static partial class Verify
{
    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/> if a condition is false.
    /// </summary>
    [DebuggerStepThrough]
    public static void Operation([DoesNotReturnIf(false)] bool condition, string? message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }

    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/> if a condition is false.
    /// </summary>
    [DebuggerStepThrough]
    public static void Operation([DoesNotReturnIf(false)] bool condition, string unformattedMessage, object? arg1)
    {
        if (!condition)
        {
            throw new InvalidOperationException(PrivateErrorHelpers.Format(unformattedMessage, arg1));
        }
    }

    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/> if a condition is false.
    /// </summary>
    [DebuggerStepThrough]
    public static void Operation([DoesNotReturnIf(false)] bool condition, string unformattedMessage, object? arg1, object? arg2)
    {
        if (!condition)
        {
            throw new InvalidOperationException(PrivateErrorHelpers.Format(unformattedMessage, arg1, arg2));
        }
    }

    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/> if a condition is false.
    /// </summary>
    [DebuggerStepThrough]
    public static void Operation([DoesNotReturnIf(false)] bool condition, string unformattedMessage, params object?[] args)
    {
        if (!condition)
        {
            throw new InvalidOperationException(PrivateErrorHelpers.Format(unformattedMessage, args));
        }
    }

    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/> if a condition is false.
    /// </summary>
    [DebuggerStepThrough]
    public static void Operation([DoesNotReturnIf(false)] bool condition, [InterpolatedStringHandlerArgument("condition")] ref ValidationInterpolatedStringHandler message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message.ToStringAndClear());
        }
    }

    /// <inheritdoc cref="Operation(bool, ResourceManager, string, object?)"/>
    /// <param name="condition"><inheritdoc cref="Operation(bool, ResourceManager, string, object?[])" path="/param[@name='condition']"/></param>
    /// <param name="resourceManager"><inheritdoc cref="Operation(bool, ResourceManager, string, object?[])" path="/param[@name='resourceManager']"/></param>
    /// <param name="resourceName"><inheritdoc cref="Operation(bool, ResourceManager, string, object?[])" path="/param[@name='unformattedMessageResourceName']"/></param>
    [DebuggerStepThrough]
    public static void Operation([DoesNotReturnIf(false)] bool condition, ResourceManager resourceManager, string resourceName)
    {
        Requires.NotNull(resourceManager, nameof(resourceManager));
        if (!condition)
        {
            throw new InvalidOperationException(resourceManager.GetString(resourceName, CultureInfo.CurrentCulture));
        }
    }

    /// <inheritdoc cref="Operation(bool, ResourceManager, string, object?, object?)"/>
    [DebuggerStepThrough]
    public static void Operation([DoesNotReturnIf(false)] bool condition, ResourceManager resourceManager, string unformattedMessageResourceName, object? arg1)
    {
        Requires.NotNull(resourceManager, nameof(resourceManager));
        if (!condition)
        {
            throw new InvalidOperationException(PrivateErrorHelpers.Format(resourceManager.GetString(unformattedMessageResourceName, CultureInfo.CurrentCulture)!, arg1));
        }
    }

    /// <inheritdoc cref="Operation(bool, ResourceManager, string, object?[])"/>
    /// <param name="condition"><inheritdoc cref="Operation(bool, ResourceManager, string, object?[])" path="/param[@name='condition']"/></param>
    /// <param name="resourceManager"><inheritdoc cref="Operation(bool, ResourceManager, string, object?[])" path="/param[@name='resourceManager']"/></param>
    /// <param name="unformattedMessageResourceName"><inheritdoc cref="Operation(bool, ResourceManager, string, object?[])" path="/param[@name='unformattedMessageResourceName']"/></param>
    /// <param name="arg1">The first formatting argument.</param>
    /// <param name="arg2">The second formatting argument.</param>
    [DebuggerStepThrough]
    public static void Operation([DoesNotReturnIf(false)] bool condition, ResourceManager resourceManager, string unformattedMessageResourceName, object? arg1, object? arg2)
    {
        Requires.NotNull(resourceManager, nameof(resourceManager));
        if (!condition)
        {
            throw new InvalidOperationException(PrivateErrorHelpers.Format(resourceManager.GetString(unformattedMessageResourceName, CultureInfo.CurrentCulture)!, arg1, arg2));
        }
    }

    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/> if a condition is false.
    /// </summary>
    /// <param name="condition">The condition to check.</param>
    /// <param name="resourceManager">The resource manager from which to retrieve the exception message. For example: <c>Strings.ResourceManager</c>.</param>
    /// <param name="unformattedMessageResourceName">The name of the string resource to obtain for the exception message. For example: <c>nameof(Strings.SomeError)</c>.</param>
    /// <param name="args">The formatting arguments.</param>
    /// <remarks>
    /// This overload allows only loading a localized string in the error condition as an optimization in perf critical sections over the simpler
    /// to use <see cref="Operation(bool, string?)"/> overload.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="resourceManager"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">Thrown if <paramref name="condition"/> is <see langword="false"/>.</exception>
    [DebuggerStepThrough]
    public static void Operation([DoesNotReturnIf(false)] bool condition, ResourceManager resourceManager, string unformattedMessageResourceName, params object?[] args)
    {
        Requires.NotNull(resourceManager, nameof(resourceManager));
        if (!condition)
        {
            throw new InvalidOperationException(PrivateErrorHelpers.Format(resourceManager.GetString(unformattedMessageResourceName, CultureInfo.CurrentCulture)!, args));
        }
    }

    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/> if a condition is false.
    /// </summary>
    [DebuggerStepThrough]
    public static void OperationWithHelp([DoesNotReturnIf(false)] bool condition, string? message, string? helpLink)
    {
        if (!condition)
        {
            var ex = new InvalidOperationException(message)
            {
                HelpLink = helpLink,
            };
            throw ex;
        }
    }

    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/>.
    /// </summary>
    /// <returns>
    /// Nothing.  This method always throws.
    /// The signature claims to return an exception to allow callers to throw this method
    /// to satisfy C# execution path constraints.
    /// </returns>
    [DebuggerStepThrough]
    [DoesNotReturn]
    public static Exception FailOperation(string message)
    {
        throw new InvalidOperationException(message);
    }

    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/>.
    /// </summary>
    /// <returns>
    /// Nothing.  This method always throws.
    /// The signature claims to return an exception to allow callers to throw this method
    /// to satisfy C# execution path constraints.
    /// </returns>
    [DebuggerStepThrough]
    [DoesNotReturn]
    public static Exception FailOperation(string message, params object?[] args)
    {
        throw new InvalidOperationException(PrivateErrorHelpers.Format(message, args));
    }

    /// <summary>
    /// Throws an <see cref="ObjectDisposedException"/> if an object is disposed.
    /// </summary>
    [DebuggerStepThrough]
    public static void NotDisposed(IDisposableObservable disposedValue, string? message = null)
    {
        Requires.NotNull(disposedValue, nameof(disposedValue));

        if (disposedValue.IsDisposed)
        {
            string objectName = disposedValue.GetType().FullName ?? string.Empty;
            if (message is object)
            {
                throw new ObjectDisposedException(objectName, message);
            }
            else
            {
                throw new ObjectDisposedException(objectName);
            }
        }
    }

    /// <summary>
    /// Throws an <see cref="ObjectDisposedException"/> if a condition is false.
    /// </summary>
    [DebuggerStepThrough]
    public static void NotDisposed([DoesNotReturnIf(false)] bool condition, object? disposedValue, string? message = null)
    {
        if (!condition)
        {
            string objectName = disposedValue is object ? (disposedValue.GetType().FullName ?? string.Empty) : string.Empty;
            if (message is object)
            {
                throw new ObjectDisposedException(objectName, message);
            }
            else
            {
                throw new ObjectDisposedException(objectName);
            }
        }
    }

    /// <summary>
    /// Throws an <see cref="ObjectDisposedException"/> if a condition is false.
    /// </summary>
    [DebuggerStepThrough]
    public static void NotDisposed([DoesNotReturnIf(false)] bool condition, string? message)
    {
        if (!condition)
        {
            throw new ObjectDisposedException(null, message);
        }
    }

    /// <summary>
    /// Throws an exception if the given value is negative.
    /// </summary>
    /// <param name="hresult">The HRESULT corresponding to the desired exception.</param>
    /// <param name="ignorePreviousComCalls">If true, prevents <c>ThrowExceptionForHR</c> from returning an exception from a previous COM call and instead always use the HRESULT specified.</param>
    /// <remarks>
    /// No exception is thrown for S_FALSE.
    /// </remarks>
    [DebuggerStepThrough]
    [System.Security.SecurityCritical]
    public static void HResult(int hresult, bool ignorePreviousComCalls = false)
    {
        if (hresult < 0)
        {
            if (ignorePreviousComCalls)
            {
                Marshal.ThrowExceptionForHR(hresult, new IntPtr(-1));
            }
            else
            {
                Marshal.ThrowExceptionForHR(hresult);
            }
        }
    }
}
