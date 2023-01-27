// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime;
using System.Runtime.CompilerServices;

namespace Microsoft;

/// <summary>
/// Common runtime checks that throw <see cref="InternalErrorException" /> exceptions upon failure.
/// </summary>
public static partial class Assumes
{
    /// <summary>
    /// Throws <see cref="InternalErrorException" /> if the specified value is null.
    /// </summary>
    /// <typeparam name="T">The type of value to test.</typeparam>
    [DebuggerStepThrough]
    [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
    public static void NotNull<T>([ValidatedNotNull, NotNull]T? value)
        where T : class
    {
        True(value is object);
    }

    /// <summary>
    /// Throws <see cref="InternalErrorException" /> if the specified value is null.
    /// </summary>
    /// <typeparam name="T">The type of value to test.</typeparam>
    [DebuggerStepThrough]
    [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
    public static void NotNull<T>([ValidatedNotNull, NotNull]T? value)
        where T : struct
    {
        True(value.HasValue);
    }

    /// <summary>
    /// Throws <see cref="InternalErrorException" /> if the specified value is null or empty.
    /// </summary>
    [DebuggerStepThrough]
    public static void NotNullOrEmpty([ValidatedNotNull, NotNull]string? value)
    {
        NotNull(value);
        True(value.Length > 0);
        True(value[0] != '\0');
    }

    /// <summary>
    /// Throws <see cref="InternalErrorException" /> if the specified value is null or empty.
    /// </summary>
    /// <typeparam name="T">The type of value to test.</typeparam>
    [DebuggerStepThrough]
    public static void NotNullOrEmpty<T>([ValidatedNotNull, NotNull]ICollection<T>? values)
    {
        Assumes.NotNull(values);
        Assumes.True(values.Count > 0);
    }

    /// <summary>
    /// Throws <see cref="InternalErrorException" /> if the specified value is null or empty.
    /// </summary>
    /// <typeparam name="T">The type of value to test.</typeparam>
    [DebuggerStepThrough]
    public static void NotNullOrEmpty<T>([ValidatedNotNull, NotNull]IEnumerable<T>? values)
    {
        Assumes.NotNull(values);

        bool isEmpty;
        if (values is ICollection<T> collection)
        {
            isEmpty = collection.Count == 0;
        }
        else if (values is IReadOnlyCollection<T> readOnlyCollection)
        {
            isEmpty = readOnlyCollection.Count == 0;
        }
        else
        {
            using IEnumerator<T> enumerator = values.GetEnumerator();
            isEmpty = !enumerator.MoveNext();
        }

        Assumes.False(isEmpty);
    }

    /// <summary>
    /// Throws <see cref="InternalErrorException" /> if the specified value is not null.
    /// </summary>
    /// <typeparam name="T">The type of value to test.</typeparam>
    [DebuggerStepThrough]
    [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
    public static void Null<T>(T? value)
        where T : class
    {
        True(value is null);
    }

    /// <summary>
    /// Throws <see cref="InternalErrorException" /> if the specified value is not null.
    /// </summary>
    /// <typeparam name="T">The type of value to test.</typeparam>
    [DebuggerStepThrough]
    [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
    public static void Null<T>(T? value)
        where T : struct
    {
        False(value.HasValue);
    }

    /// <summary>
    /// Throws <see cref="InternalErrorException" /> if the specified object is not of a given type.
    /// </summary>
    /// <typeparam name="T">The type the value is expected to be.</typeparam>
    /// <param name="value">The value to test.</param>
    [DebuggerStepThrough]
    [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
    public static void Is<T>([NotNull] object? value)
    {
        True(value is T);
    }

    /// <summary>
    /// Throws an public exception if a condition evaluates to true.
    /// </summary>
    [DebuggerStepThrough]
    [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
    public static void False([DoesNotReturnIf(true)] bool condition, [CallerArgumentExpression(nameof(condition)), Localizable(false)] string? message = null)
    {
        if (condition)
        {
            Fail(message);
        }
    }

    /// <summary>
    /// Throws an public exception if a condition evaluates to true.
    /// </summary>
    [DebuggerStepThrough]
    [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
    public static void False([DoesNotReturnIf(true)] bool condition, [Localizable(false)] string unformattedMessage, object? arg1)
    {
        if (condition)
        {
            Fail(Format(unformattedMessage, arg1));
        }
    }

    /// <summary>
    /// Throws an public exception if a condition evaluates to true.
    /// </summary>
    [DebuggerStepThrough]
    [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
    public static void False([DoesNotReturnIf(true)] bool condition, [Localizable(false)] string unformattedMessage, params object?[] args)
    {
        if (condition)
        {
            Fail(Format(unformattedMessage, args));
        }
    }

    /// <summary>
    /// Throws an public exception if a condition evaluates to false.
    /// </summary>
    [DebuggerStepThrough]
    [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
    public static void True([DoesNotReturnIf(false)] bool condition, [CallerArgumentExpression(nameof(condition)), Localizable(false)] string? message = null)
    {
        if (!condition)
        {
            Fail(message);
        }
    }

    /// <summary>
    /// Throws an public exception if a condition evaluates to false.
    /// </summary>
    [DebuggerStepThrough]
    [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
    public static void True([DoesNotReturnIf(false)] bool condition, [Localizable(false)] string unformattedMessage, object? arg1)
    {
        if (!condition)
        {
            Fail(Format(unformattedMessage, arg1));
        }
    }

    /// <summary>
    /// Throws an public exception if a condition evaluates to false.
    /// </summary>
    [DebuggerStepThrough]
    [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
    public static void True([DoesNotReturnIf(false)] bool condition, [Localizable(false)] string unformattedMessage, params object?[] args)
    {
        if (!condition)
        {
            Fail(Format(unformattedMessage, args));
        }
    }

    /// <summary>
    /// Unconditionally throws an <see cref="InternalErrorException"/>.
    /// </summary>
    /// <returns>Nothing. This method always throws.</returns>
    [DebuggerStepThrough]
    [DoesNotReturn]
    public static Exception NotReachable()
    {
        // Keep these two as separate lines of code, so the debugger can come in during the assert dialog
        // that the exception's constructor displays, and the debugger can then be made to skip the throw
        // in order to continue the investigation.
        var exception = new InternalErrorException();
        bool proceed = true; // allows debuggers to skip the throw statement
        if (proceed)
        {
            throw exception;
        }
        else
        {
#pragma warning disable CS8763
            return new Exception();
#pragma warning restore CS8763
        }
    }

    /// <summary>
    /// Unconditionally throws an <see cref="InternalErrorException"/>.
    /// </summary>
    /// <typeparam name="T">The type that the method should be typed to return (although it never returns anything).</typeparam>
    /// <returns>Nothing. This method always throws.</returns>
    [DebuggerStepThrough]
    [DoesNotReturn]
    [return: MaybeNull]
    public static T NotReachable<T>()
    {
        // Keep these two as separate lines of code, so the debugger can come in during the assert dialog
        // that the exception's constructor displays, and the debugger can then be made to skip the throw
        // in order to continue the investigation.
        var exception = new InternalErrorException();
        bool proceed = true; // allows debuggers to skip the throw statement
        if (proceed)
        {
            throw exception;
        }
        else
        {
#pragma warning disable CS8763 // A method marked [DoesNotReturn] should not return.
            return default;
#pragma warning restore CS8763 // A method marked [DoesNotReturn] should not return.
        }
    }

    /// <summary>
    /// Verifies that a value is not null, and throws <see cref="InternalErrorException" /> about a missing service otherwise.
    /// </summary>
    /// <typeparam name="T">The interface of the imported part.</typeparam>
    [DebuggerStepThrough]
    public static void Present<T>([NotNull]T component)
    {
        if (component is null)
        {
            Type coreType = PrivateErrorHelpers.TrimGenericWrapper(typeof(T), typeof(Lazy<>));
            Fail(string.Format(CultureInfo.CurrentCulture, Strings.ServiceMissing, coreType.FullName));
        }
    }

    /// <summary>
    /// Throws an public exception.
    /// </summary>
    /// <returns>Nothing, as this method always throws.  The signature allows for "throwing" Fail so C# knows execution will stop.</returns>
    [DebuggerStepThrough]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use Fail(string) instead.")]
    [DoesNotReturn]
    public static Exception Fail([Localizable(false)] string? message = null, bool showAssert = true) => Fail(message);

    /// <summary>
    /// Throws an public exception.
    /// </summary>
    /// <returns>Nothing, as this method always throws.  The signature allows for "throwing" Fail so C# knows execution will stop.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use Fail(string, Exception) instead.")]
    [DoesNotReturn]
    public static Exception Fail([Localizable(false)] string? message, Exception? innerException, bool showAssert = true) => Fail(message, innerException);

    /// <summary>
    /// Throws an public exception.
    /// </summary>
    /// <returns>Nothing, as this method always throws.  The signature allows for "throwing" Fail so C# knows execution will stop.</returns>
    [DebuggerStepThrough]
    [DoesNotReturn]
    public static Exception Fail([Localizable(false)] string? message = null)
    {
        var exception = new InternalErrorException(message);
        bool proceed = true; // allows debuggers to skip the throw statement
        if (proceed)
        {
            throw exception;
        }
        else
        {
#pragma warning disable CS8763
            return new Exception();
#pragma warning restore CS8763
        }
    }

    /// <summary>
    /// Throws an public exception.
    /// </summary>
    /// <returns>Nothing, as this method always throws.  The signature allows for "throwing" Fail so C# knows execution will stop.</returns>
    [DoesNotReturn]
    public static Exception Fail([Localizable(false)] string? message, Exception? innerException)
    {
        var exception = new InternalErrorException(message, innerException);
        bool proceed = true; // allows debuggers to skip the throw statement
        if (proceed)
        {
            throw exception;
        }
        else
        {
#pragma warning disable CS8763
            return new Exception();
#pragma warning restore CS8763
        }
    }

    /// <summary>
    /// Helper method that formats string arguments.
    /// </summary>
    private static string Format(string format, params object?[] arguments)
    {
        return PrivateErrorHelpers.Format(format, arguments);
    }
}
