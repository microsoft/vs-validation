// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

internal class DisposableValue<T> : IDisposable
{
    private Action? disposeAction;

    internal DisposableValue(T value, Action disposeAction)
    {
        this.Value = value;
        this.disposeAction = disposeAction;
    }

    internal T Value { get; }

    public void Dispose()
    {
        this.disposeAction?.Invoke();
        this.disposeAction = null;
    }
}
