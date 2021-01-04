using System;
using System.Linq;
using Xunit;

namespace MSJennings.CodeGeneration.Tests
{
    public class CollectionExtensionsTests
    {
        [Fact]
        public void Concat_WithIEnumerableStringAndStringParams_ShouldConcatenate()
        {
            // Arrange
            var originalList = new[] { "One", "Two", "Three" };

            // Act
            var result = originalList.Concat("Four", "Five");

            // Assert
            Assert.Equal(5, result.Count());
            Assert.Contains(result, x => x.Equals("Four", StringComparison.Ordinal));
            Assert.Contains(result, x => x.Equals("Five", StringComparison.Ordinal));
        }
    }
}
