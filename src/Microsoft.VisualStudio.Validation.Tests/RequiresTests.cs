namespace Microsoft.VisualStudio.Validation.Tests
{
    using System;
    using Xunit;

    public class RequiresTests
    {
        [Fact]
        public void NotNull_ThrowsOnNull()
        {
            Assert.Throws<ArgumentNullException>(() => Requires.NotNull<object>((object)null, "foo"));
        }
    }
}
