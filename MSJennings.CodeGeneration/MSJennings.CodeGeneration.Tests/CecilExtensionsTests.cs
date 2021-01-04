using Mono.Cecil;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;

namespace MSJennings.CodeGeneration.Tests
{
    public class CecilExtensionsTests
    {
        private static AssemblyDefinition LoadTestAssembly()
        {
            // todo: add pre-build steps to build the test assembly and copy output to a "test assets" folder or similar
            var thisAssemblyUri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            var thisAssemblyPath = Uri.UnescapeDataString(thisAssemblyUri.AbsolutePath);
            var thisAssemblyDirectory = Path.GetDirectoryName(thisAssemblyPath);
            var solutionDirectory = Path.Combine(thisAssemblyDirectory, "..", "..", "..", "..");
            var testAssemblyDirectory = Path.Combine(solutionDirectory, "MSJennings.CodeGeneration.Tests.TestAssembly", "bin", "Debug", "netstandard2.1");
            var testAssemblyFileName = Path.Combine(testAssemblyDirectory, "MSJennings.CodeGeneration.Tests.TestAssembly.dll");

            var assemblyDefinition = AssemblyDefinition.ReadAssembly(testAssemblyFileName);
            return assemblyDefinition;
        }

        private static TypeDefinition GetTypeDefinition(AssemblyDefinition assemblyDefintion, string typeName) =>
            assemblyDefintion.MainModule.Types.FirstOrDefault(x => x.Name.Equals(typeName, StringComparison.Ordinal))
            ?? throw new ArgumentOutOfRangeException(nameof(typeName), $"Type name '{typeName}' was not found");

        private static PropertyDefinition GetPropertyDefinition(TypeDefinition typeDefinition, string propertyName) =>
            typeDefinition.Properties.FirstOrDefault(x => x.Name.Equals(propertyName, StringComparison.Ordinal))
            ?? throw new ArgumentOutOfRangeException(nameof(propertyName), $"Property name '{propertyName}' was not found");

        [Fact]
        public void ToModelPropertyType_WithBool_ShouldReturnExpectedResult()
        {
            // Arrange
            var assemblyDefinition = LoadTestAssembly();
            var typeDefinition = GetTypeDefinition(assemblyDefinition, "Quiz");
            var propertyDefintion = GetPropertyDefinition(typeDefinition, "IsActive");

            // Act
            var modelPropertyType = propertyDefintion.PropertyType.ToModelPropertyType();

            // Assert
            Assert.Equal(ModelPropertyLogicalType.Boolean, modelPropertyType.LogicalType);
            Assert.Null(modelPropertyType.ObjectTypeName);
            Assert.Null(modelPropertyType.ListItemType);
        }

        [Fact]
        public void ToModelPropertyType_WithListOfStrings_ShouldReturnExpectedResult()
        {
            // Arrange
            var assemblyDefinition = LoadTestAssembly();
            var typeDefinition = GetTypeDefinition(assemblyDefinition, "Quiz");
            var propertyDefintion = GetPropertyDefinition(typeDefinition, "Topics");

            // Act
            var modelPropertyType = propertyDefintion.PropertyType.ToModelPropertyType();

            // Assert
            Assert.Equal(ModelPropertyLogicalType.List, modelPropertyType.LogicalType);
            Assert.Null(modelPropertyType.ObjectTypeName);
            Assert.Equal(ModelPropertyLogicalType.String, modelPropertyType.ListItemType.LogicalType);
        }

        [Fact]
        public void ToModelPropertyType_WithArrayOfInts_ShouldReturnExpectedResult()
        {
            // Arrange
            var assemblyDefinition = LoadTestAssembly();
            var typeDefinition = GetTypeDefinition(assemblyDefinition, "Question");
            var propertyDefintion = GetPropertyDefinition(typeDefinition, "QuizIds");

            // Act
            var modelPropertyType = propertyDefintion.PropertyType.ToModelPropertyType();

            // Assert
            Assert.Equal(ModelPropertyLogicalType.List, modelPropertyType.LogicalType);
            Assert.Null(modelPropertyType.ObjectTypeName);
            Assert.Equal(ModelPropertyLogicalType.Integer, modelPropertyType.ListItemType.LogicalType);
        }

        [Fact]
        public void ToModelPropertyType_WithDictionary_ShouldReturnExpectedResult()
        {
            // Arrange
            var assemblyDefinition = LoadTestAssembly();
            var typeDefinition = GetTypeDefinition(assemblyDefinition, "Question");
            var propertyDefintion = GetPropertyDefinition(typeDefinition, "Choices");

            // Act
            var modelPropertyType = propertyDefintion.PropertyType.ToModelPropertyType();

            // Assert
            Assert.Equal(ModelPropertyLogicalType.List, modelPropertyType.LogicalType);
            Assert.Null(modelPropertyType.ObjectTypeName);
            Assert.Equal(ModelPropertyLogicalType.KeyValuePair, modelPropertyType.ListItemType.LogicalType);
        }

        [Fact]
        public void ToModelPropertyType_WithObject_ShouldReturnExpectedResult()
        {
            // Arrange
            var assemblyDefinition = LoadTestAssembly();
            var typeDefinition = GetTypeDefinition(assemblyDefinition, "Question");

            // Act
            var modelPropertyType = typeDefinition.ToModelPropertyType();

            // Assert
            Assert.Equal(ModelPropertyLogicalType.Object, modelPropertyType.LogicalType);
            Assert.Equal(typeDefinition.Name, modelPropertyType.ObjectTypeName);
            Assert.Null(modelPropertyType.ListItemType);
        }

    }
}
