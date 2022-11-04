// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft;
using Xunit;

public class EventHandlerExtensionsTests
{
    private static readonly EventArgs Args = new EventArgs();

    [Fact]
    public void Raise_EventHandlerOfT()
    {
        bool invoked = false;
        EventHandler<EventArgs>? handler = (s, e) =>
        {
            Assert.Same(this, s);
            Assert.Same(Args, e);
            invoked = true;
        };
        handler.Raise(this, Args);
        Assert.True(invoked);

        Assert.Throws<ArgumentNullException>(() => handler.Raise(null!, EventArgs.Empty));
        Assert.Throws<ArgumentNullException>(() => handler.Raise(this, null!));

        handler = null;
        handler.Raise(this, Args);
        Assert.Throws<ArgumentNullException>(() => handler.Raise(null!, EventArgs.Empty));
        Assert.Throws<ArgumentNullException>(() => handler.Raise(this, null!));
    }

    [Fact]
    public void Raise_EventHandler()
    {
        bool invoked = false;
        EventHandler? handler = (s, e) =>
        {
            Assert.Same(this, s);
            Assert.Same(Args, e);
            invoked = true;
        };
        handler.Raise(this, Args);
        Assert.True(invoked);

        Assert.Throws<ArgumentNullException>(() => handler.Raise(null!, EventArgs.Empty));
        Assert.Throws<ArgumentNullException>(() => handler.Raise(this, null!));

        handler = null;
        handler.Raise(this, Args);
        Assert.Throws<ArgumentNullException>(() => handler.Raise(null!, EventArgs.Empty));
        Assert.Throws<ArgumentNullException>(() => handler.Raise(this, null!));
    }

    [Fact]
    public void Raise_Delegate()
    {
        bool invoked = false;
        Delegate? handler = new EventHandler((s, e) =>
        {
            Assert.Same(this, s);
            Assert.Same(Args, e);
            invoked = true;
        });
        handler.Raise(this, Args);
        Assert.True(invoked);

        Assert.Throws<ArgumentNullException>(() => handler.Raise(null!, EventArgs.Empty));
        Assert.Throws<ArgumentNullException>(() => handler.Raise(this, null!));

        handler = null;
        handler.Raise(this, Args);
        Assert.Throws<ArgumentNullException>(() => handler.Raise(null!, EventArgs.Empty));
        Assert.Throws<ArgumentNullException>(() => handler.Raise(this, null!));
    }
}
