using System.Collections.Generic;
using Xunit;

namespace MSJennings.CodeGeneration.Tests
{
    public class ReflectionExtensionTests
    {
        [Fact]
        public void ToModelPropertyType_WithBool_ShouldReturnExpectedResult()
        {
            // Arrange
            var testValue = true;
            var testValueType = testValue.GetType();

            // Act
            var modelPropertyType = testValueType.ToModelPropertyType();

            // Assert
            Assert.Equal(ModelPropertyLogicalType.Boolean, modelPropertyType.LogicalType);
            Assert.Null(modelPropertyType.ObjectTypeName);
            Assert.Null(modelPropertyType.ListItemType);
        }

        [Fact]
        public void ToModelPropertyType_WithListOfStrings_ShouldReturnExpectedResult()
        {
            // Arrange
            var testValue = new List<string> { "One", "Two", "Three" };
            var testValueType = testValue.GetType();

            // Act
            var modelPropertyType = testValueType.ToModelPropertyType();

            // Assert
            Assert.Equal(ModelPropertyLogicalType.List, modelPropertyType.LogicalType);
            Assert.Null(modelPropertyType.ObjectTypeName);
            Assert.Equal(ModelPropertyLogicalType.String, modelPropertyType.ListItemType.LogicalType);
        }

        [Fact]
        public void ToModelPropertyType_WithArrayOfInts_ShouldReturnExpectedResult()
        {
            // Arrange
            var testValue = new int[] { 1, 2, 3 };
            var testValueType = testValue.GetType();

            // Act
            var modelPropertyType = testValueType.ToModelPropertyType();

            // Assert
            Assert.Equal(ModelPropertyLogicalType.List, modelPropertyType.LogicalType);
            Assert.Null(modelPropertyType.ObjectTypeName);
            Assert.Equal(ModelPropertyLogicalType.Integer, modelPropertyType.ListItemType.LogicalType);
        }

        [Fact]
        public void ToModelPropertyType_WithDictionary_ShouldReturnExpectedResult()
        {
            // Arrange
            var testValue = new Dictionary<int, string>
            {
                {1, "One"},
                {2, "Two"},
                {3, "Three"},
            };

            var testValueType = testValue.GetType();

            // Act
            var modelPropertyType = testValueType.ToModelPropertyType();

            // Assert
            Assert.Equal(ModelPropertyLogicalType.List, modelPropertyType.LogicalType);
            Assert.Null(modelPropertyType.ObjectTypeName);
            Assert.Equal(ModelPropertyLogicalType.KeyValuePair, modelPropertyType.ListItemType.LogicalType);
        }

        [Fact]
        public void ToModelPropertyType_WithObject_ShouldReturnExpectedResult()
        {
            // Arrange
            var testValue = new CodeGenerationModel();
            var testValueType = testValue.GetType();

            // Act
            var modelPropertyType = testValueType.ToModelPropertyType();

            // Assert
            Assert.Equal(ModelPropertyLogicalType.Object, modelPropertyType.LogicalType);
            Assert.Equal(testValueType.Name, modelPropertyType.ObjectTypeName);
            Assert.Null(modelPropertyType.ListItemType);
        }
    }
}
