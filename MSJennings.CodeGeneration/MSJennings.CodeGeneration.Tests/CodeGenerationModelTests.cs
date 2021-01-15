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
        public void ToJson_WithAdHocModel_ShouldReturnExpectedJsonString()
        {
            // arrange
            var model = new CodeGenerationModel()
                .SetCurrentNamespace("MSJennings.Quizzes")

                .AddEntity("Quiz")
                .AddProperty("Id", ModelPropertyLogicalType.String, isRequired: true)
                .AddProperty("Name", ModelPropertyLogicalType.String, isRequired: true)
                .AddProperty("PassingScore", ModelPropertyLogicalType.Integer, isRequired: true)

                .AddEntity("Question")
                .AddProperty("Id", ModelPropertyLogicalType.String, isRequired: true)
                .AddProperty("QuizId", ModelPropertyLogicalType.String, isRequired: true)
                .AddProperty("Prompt", ModelPropertyLogicalType.String, isRequired: true)
                .AddProperty("CorrectAnswer", ModelPropertyLogicalType.String, isRequired: true)

                .AddEntity("PossibleAnswer")
                .AddProperty("Id", ModelPropertyLogicalType.String, isRequired: true)
                .AddProperty("QuestionId", ModelPropertyLogicalType.String, isRequired: true)
                .AddProperty("Value", ModelPropertyLogicalType.String, isRequired: true);

            // act
            var result = model.ToJson();

            // assert
            Assert.NotNull(result);
            Assert.True(!string.IsNullOrWhiteSpace(result));
        }

        [Fact]
        public void LoadFromTypes_WithListOfTypes_ShouldReturnExpectedJsonString()
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

            // assert that the number of namespaces is correct
            Assert.Equal(1, model.Namespaces.Count);

            // assert that the number of enties is correct
            Assert.Equal(2, model.Entities.Count());

            // assert that the expected entities are present
            Assert.Contains(model.Entities, x => x.Name.Equals(nameof(Quiz), StringComparison.Ordinal));
            Assert.Contains(model.Entities, x => x.Name.Equals(nameof(Question), StringComparison.Ordinal));
            
            var quizEntity = model.Entities.First(x => x.Name.Equals(nameof(Quiz), StringComparison.Ordinal));
            var questionEntity = model.Entities.First(x => x.Name.Equals(nameof(Question), StringComparison.Ordinal));

            // assert that entities have the expected number of properties
            Assert.Equal(7, quizEntity.Properties.Count);
            Assert.Equal(5, questionEntity.Properties.Count);

            // assert that entities have the expected properties and property types
            Assert.Contains(quizEntity.Properties, x =>
                x.Name.Equals(nameof(Quiz.Id), StringComparison.Ordinal) &&
                x.PropertyType.LogicalType == ModelPropertyLogicalType.Integer);

            Assert.Contains(quizEntity.Properties, x =>
                x.Name.Equals(nameof(Quiz.Name), StringComparison.Ordinal) &&
                x.PropertyType.LogicalType == ModelPropertyLogicalType.String);

            Assert.Contains(quizEntity.Properties, x =>
                x.Name.Equals(nameof(Quiz.CreatedDate), StringComparison.Ordinal) &&
                x.PropertyType.LogicalType == ModelPropertyLogicalType.DateAndTime);

            Assert.Contains(quizEntity.Properties, x =>
                x.Name.Equals(nameof(Quiz.IsActive), StringComparison.Ordinal) &&
                x.PropertyType.LogicalType == ModelPropertyLogicalType.Boolean);

            Assert.Contains(quizEntity.Properties, x =>
                x.Name.Equals(nameof(Quiz.PassingScore), StringComparison.Ordinal) &&
                x.PropertyType.LogicalType == ModelPropertyLogicalType.Decimal);

            Assert.Contains(quizEntity.Properties, x =>
                x.Name.Equals(nameof(Quiz.Topics), StringComparison.Ordinal) &&
                x.PropertyType.LogicalType == ModelPropertyLogicalType.List &&
                x.PropertyType.ListItemType.LogicalType == ModelPropertyLogicalType.String);

            Assert.Contains(quizEntity.Properties, x =>
                x.Name.Equals(nameof(Quiz.Questions), StringComparison.Ordinal) &&
                x.PropertyType.LogicalType == ModelPropertyLogicalType.List &&
                x.PropertyType.ListItemType.ObjectTypeName.Equals(nameof(Question), StringComparison.Ordinal));

            Assert.Contains(questionEntity.Properties, x =>
                x.Name.Equals(nameof(Question.Choices), StringComparison.Ordinal) &&
                x.PropertyType.LogicalType == ModelPropertyLogicalType.List &&
                x.PropertyType.ListItemType.LogicalType == ModelPropertyLogicalType.KeyValuePair);

            Assert.Contains(questionEntity.Properties, x =>
                x.Name.Equals(nameof(Question.CorrectChoice), StringComparison.Ordinal) &&
                x.PropertyType.LogicalType == ModelPropertyLogicalType.Character);

            Assert.Contains(questionEntity.Properties, x =>
                x.Name.Equals(nameof(Question.QuizIds), StringComparison.Ordinal) &&
                x.PropertyType.LogicalType == ModelPropertyLogicalType.List &&
                x.PropertyType.ListItemType.LogicalType == ModelPropertyLogicalType.Integer);
        }
    }
}
