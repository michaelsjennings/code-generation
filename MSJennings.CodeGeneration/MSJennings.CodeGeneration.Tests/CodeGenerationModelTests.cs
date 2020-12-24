using Xunit;

namespace MSJennings.CodeGeneration.Tests
{

    public class CodeGenerationModelTests
    {
        [Fact]
        public void ToJson_WithQuizModel_ShouldReturnExpectedJsonString()
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
    }
}
