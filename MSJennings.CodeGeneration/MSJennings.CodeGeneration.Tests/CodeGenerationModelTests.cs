using MSJennings.CodeGeneration.Tests.TestAssembly.Quizzes;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MSJennings.CodeGeneration.Tests
{
    public class CodeGenerationModelTests
    {
        [Fact]
        public void FluentModelBuilder_WithAdHocModel_ShouldBuildExpectedModel()
        {
            // arrange

            // act
            var model = new CodeGenerationModel()
                .SetCurrentNamespace("MSJennings.Quizzes")

                .AddEntity("Quiz")
                .AddProperty("Id", ModelPropertyLogicalType.Integer, isRequired: true)
                .AddProperty("Name", ModelPropertyLogicalType.String)
                .AddProperty("CreatedDate", ModelPropertyLogicalType.DateAndTime)
                .AddProperty("IsActive", ModelPropertyLogicalType.Boolean)
                .AddProperty("PassingScore", ModelPropertyLogicalType.Decimal)
                .AddListProperty("Topics", ModelPropertyLogicalType.String)
                .AddListProperty("Questions", "Question", isRequired: true)

                .AddEntity("Question")
                .AddProperty("Id", ModelPropertyLogicalType.Integer)
                .AddProperty("Prompt", ModelPropertyLogicalType.String)
                .AddListProperty("Choices", ModelPropertyLogicalType.KeyValuePair, isRequired: true)
                .AddProperty("CorrectChoice", ModelPropertyLogicalType.Character)
                .AddListProperty("QuizIds", ModelPropertyLogicalType.Integer);

            // assert
            AssertIsValidModel(model);
        }

        [Fact]
        public void LoadFromTypes_WithListOfTypes_ShouldLoadExpectedModel()
        {
            // arrange
            var model = new CodeGenerationModel();

            var types = new List<Type>
            {
                typeof(Quiz),
                typeof(Question)
            };

            // act
            model.LoadFromTypes(types);

            // assert
            AssertIsValidModel(model);
        }

        [Fact]
        public void LoadFromAssembly_WithAssemblyFileName_ShouldLoadExpectedModel()
        {
            // arrange
            var model = new CodeGenerationModel();

            var testAssembly = typeof(Quiz).Assembly;
            var testAssemblyFileName = testAssembly.Location;

            // act
            model.LoadFromAssembly(testAssemblyFileName);

            // assert
            AssertIsValidModel(model);
        }

        [Fact]
        public void LoadFromOtherModel_WithExistingModel_ShouldLoadExpectedModel()
        {
            // arrange
            var originalModel = new CodeGenerationModel();

            originalModel.LoadFromTypes(
                typeof(Quiz),
                typeof(Question));

            var newModel = new CodeGenerationModel();

            // act
            newModel.LoadFromOtherModel(originalModel);

            // assert
            AssertIsValidModel(newModel);
        }

        [Fact]
        public void LoadFromJson_WithExistingModelJson_ShouldLoadExpectedModel()
        {
            // arrange
            var originalModel = new CodeGenerationModel();

            originalModel.LoadFromTypes(
                typeof(Quiz),
                typeof(Question));

            var originalModelJson = originalModel.ToJson();

            var newModel = new CodeGenerationModel();

            // act
            newModel.LoadFromJson(originalModelJson);

            // assert
            AssertIsValidModel(newModel);
        }

        private void AssertIsValidModel(CodeGenerationModel model)
        {
            Assert.NotNull(model);
            Assert.Equal(1, model.Namespaces.Count);

            // assert that the number of enties is correct
            Assert.Equal(2, model.Entities.Count());

            // assert that the expected entities are present
            Assert.Contains(model.Entities, x => x.Name.Equals("Quiz", StringComparison.Ordinal));
            Assert.Contains(model.Entities, x => x.Name.Equals("Question", StringComparison.Ordinal));

            var quizEntity = model.Entities.First(x => x.Name.Equals("Quiz", StringComparison.Ordinal));
            var questionEntity = model.Entities.First(x => x.Name.Equals("Question", StringComparison.Ordinal));

            // assert that entities have the expected number of properties
            Assert.Equal(7, quizEntity.Properties.Count);
            Assert.Equal(5, questionEntity.Properties.Count);

            // assert that entities have the expected properties and property types
            Assert.Contains(quizEntity.Properties, x =>
                x.Name.Equals("Id", StringComparison.Ordinal) &&
                x.PropertyType.LogicalType == ModelPropertyLogicalType.Integer &&
                x.IsRequired);

            Assert.Contains(quizEntity.Properties, x =>
                x.Name.Equals("Name", StringComparison.Ordinal) &&
                x.PropertyType.LogicalType == ModelPropertyLogicalType.String &&
                !x.IsRequired);

            Assert.Contains(quizEntity.Properties, x =>
                x.Name.Equals("CreatedDate", StringComparison.Ordinal) &&
                x.PropertyType.LogicalType == ModelPropertyLogicalType.DateAndTime &&
                !x.IsRequired);

            Assert.Contains(quizEntity.Properties, x =>
                x.Name.Equals("IsActive", StringComparison.Ordinal) &&
                x.PropertyType.LogicalType == ModelPropertyLogicalType.Boolean &&
                !x.IsRequired);

            Assert.Contains(quizEntity.Properties, x =>
                x.Name.Equals("PassingScore", StringComparison.Ordinal) &&
                x.PropertyType.LogicalType == ModelPropertyLogicalType.Decimal &&
                !x.IsRequired);

            Assert.Contains(quizEntity.Properties, x =>
                x.Name.Equals("Topics", StringComparison.Ordinal) &&
                x.PropertyType.LogicalType == ModelPropertyLogicalType.List &&
                x.PropertyType.ListItemType.LogicalType == ModelPropertyLogicalType.String &&
                !x.IsRequired);

            Assert.Contains(quizEntity.Properties, x =>
                x.Name.Equals("Questions", StringComparison.Ordinal) &&
                x.PropertyType.LogicalType == ModelPropertyLogicalType.List &&
                x.PropertyType.ListItemType.ObjectTypeName.Equals("Question", StringComparison.Ordinal) &&
                x.IsRequired);

            Assert.Contains(questionEntity.Properties, x =>
                x.Name.Equals("Choices", StringComparison.Ordinal) &&
                x.PropertyType.LogicalType == ModelPropertyLogicalType.List &&
                x.PropertyType.ListItemType.LogicalType == ModelPropertyLogicalType.KeyValuePair &&
                x.IsRequired);

            Assert.Contains(questionEntity.Properties, x =>
                x.Name.Equals("CorrectChoice", StringComparison.Ordinal) &&
                x.PropertyType.LogicalType == ModelPropertyLogicalType.Character &&
                !x.IsRequired);

            Assert.Contains(questionEntity.Properties, x =>
                x.Name.Equals("QuizIds", StringComparison.Ordinal) &&
                x.PropertyType.LogicalType == ModelPropertyLogicalType.List &&
                x.PropertyType.ListItemType.LogicalType == ModelPropertyLogicalType.Integer &&
                !x.IsRequired);
        }
    }
}
