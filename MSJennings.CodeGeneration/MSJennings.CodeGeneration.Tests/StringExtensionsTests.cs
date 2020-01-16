using System;
using System.Linq;
using Xunit;

namespace MSJennings.CodeGeneration.Tests
{
    public class StringExtensionsTests
    {
        [Fact]
        public void ToWordsList_WithComplexString_ShouldReturnExpectedResult()
        {
            // arrange
            var input = "My123FileHTTPPath4";
            var expected = new[] { "My", "123", "File", "HTTP", "Path", "4" };

            // act
            var result = input.ToWordsList();

            // assert
            Assert.True(result.SequenceEqual(expected));
        }

        [Theory]
        [InlineData("employee", "employee")]
        [InlineData("Employee", "employee")]
        [InlineData("MyEmployee", "myEmployee")]
        public void ToCamelCase_WithVariousTestCases_ShouldReturnExpectedResult(string input, string expected)
        {
            // arrange

            // act
            var result = input.ToCamelCase();

            // assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("Employee", "Employee")]
        [InlineData("employee", "Employee")]
        [InlineData("myEmployee", "MyEmployee")]
        public void ToPascalCase_WithVariousTestCases_ShouldReturnExpectedResult(string input, string expected)
        {
            // arrange

            // act
            var result = input.ToPascalCase();

            // assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("employee", "employee")]
        [InlineData("employees", "employee")]
        [InlineData("myEmployees", "myEmployee")]
        public void ToSingular_WithVariousTestCases_ShouldReturnExpectedResult(string input, string expected)
        {
            // arrange

            // act
            var result = input.ToSingular();

            // assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("employees", "employees")]
        [InlineData("employee", "employees")]
        [InlineData("myEmployee", "myEmployees")]
        public void ToPlural_WithVariousTestCases_ShouldReturnExpectedResult(string input, string expected)
        {
            // arrange

            // act
            var result = input.ToPlural();

            // assert
            Assert.Equal(expected, result);
        }
    }
}
