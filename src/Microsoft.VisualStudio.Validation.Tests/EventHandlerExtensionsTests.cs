// <copyright file="EventHandlerExtensionsTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Microsoft.VisualStudio.Validation.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class EventHandlerExtensionsTests
    {
        private static readonly EventArgs Args = new EventArgs();

        [Fact]
        public void Raise_EventHandlerOfT()
        {
            bool invoked = false;
            EventHandler<EventArgs> handler = (s, e) =>
            {
                Assert.Same(this, s);
                Assert.Same(Args, e);
                invoked = true;
            };
            handler.Raise(this, Args);
            Assert.True(invoked);

            Assert.Throws<ArgumentNullException>(() => handler.Raise(null, EventArgs.Empty));
            Assert.Throws<ArgumentNullException>(() => handler.Raise(this, null));
        }

        [Fact]
        public void Raise_EventHandler()
        {
            bool invoked = false;
            EventHandler handler = (s, e) =>
            {
                Assert.Same(this, s);
                Assert.Same(Args, e);
                invoked = true;
            };
            handler.Raise(this, Args);
            Assert.True(invoked);

            Assert.Throws<ArgumentNullException>(() => handler.Raise(null, EventArgs.Empty));
            Assert.Throws<ArgumentNullException>(() => handler.Raise(this, null));
        }

        [Fact]
        public void Raise_Delegate()
        {
            bool invoked = false;
            Delegate handler = new EventHandler((s, e) =>
            {
                Assert.Same(this, s);
                Assert.Same(Args, e);
                invoked = true;
            });
            handler.Raise(this, Args);
            Assert.True(invoked);

            Assert.Throws<ArgumentNullException>(() => handler.Raise(null, EventArgs.Empty));
            Assert.Throws<ArgumentNullException>(() => handler.Raise(this, null));
        }
    }
}
