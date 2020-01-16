using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MSJennings.CodeGeneration.Tests
{
    public class StringBuilderExtensionsTests
    {
        [Fact]
        public void EndsWithNewLine_WithEmptyStringBuilder_ShouldReturnFalse()
        {
            // arrange
            var sb = new StringBuilder();

            // act
            var result = sb.EndsWithNewLine();

            // assert
            Assert.False(result);
        }

        [Fact]
        public void EndsWithNewLine_WithStringBuilderEndingWithText_ShouldReturnFalse()
        {
            // arrange
            var sb = new StringBuilder();
            sb.Append("Test");

            // act
            var result = sb.EndsWithNewLine();

            // assert
            Assert.False(result);
        }

        [Fact]
        public void EndsWithNewLine_WithStringBuilderEndingWithSpace_ShouldReturnFalse()
        {
            // arrange
            var sb = new StringBuilder();
            sb.Append("Test ");

            // act
            var result = sb.EndsWithNewLine();

            // assert
            Assert.False(result);
        }


        [Fact]
        public void EndsWithNewLine_WithStringBuilderEndingWithEnvironmentNewLine_ShouldReturnTrue()
        {
            // arrange
            var sb = new StringBuilder();
            sb.AppendLine("Test");

            // act
            var result = sb.EndsWithNewLine();

            // assert
            Assert.True(result);
        }

        [Fact]
        public void EndsWithNewLine_WithStringBuilderEndingWithEscapeRN_ShouldReturnTrue()
        {
            // arrange
            var sb = new StringBuilder();
            sb.Append("Test\r\n");

            // act
            var result = sb.EndsWithNewLine();

            // assert
            Assert.True(result);
        }

        [Fact]
        public void EndsWithNewLine_WithStringBuilderEndingWithEscapeN_ShouldReturnTrue()
        {
            // arrange
            var sb = new StringBuilder();
            sb.Append("Test\n");

            // act
            var result = sb.EndsWithNewLine();

            // assert
            Assert.True(result);
        }

        [Fact]
        public void EndsWithNewLine_WithStringBuilderEndingWithEscapeR_ShouldReturnFalse()
        {
            // arrange
            var sb = new StringBuilder();
            sb.Append("Test\r");

            // act
            var result = sb.EndsWithNewLine();

            // assert
            Assert.False(result);
        }
    }
}
