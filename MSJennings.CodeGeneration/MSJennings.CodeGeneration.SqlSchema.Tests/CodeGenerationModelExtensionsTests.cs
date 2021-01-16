using MSJennings.SqlSchema;
using System;
using System.Linq;
using Xunit;

namespace MSJennings.CodeGeneration.SqlSchema.Tests
{
    public class CodeGenerationModelExtensionsTests
    {
        [Fact]
        public void LoadFromSqlDatabase_WithSqlDatabase_ShouldLoadExpectedModel()
        {
            // Arrange
            var database = new SqlDatabase();

            database.Tables.Add(new SqlTable("Quiz") { Database = database, Schema = new SqlSchemaName("Quizzes") });
            var quizTable = database.Tables.Get("Quizzes", "Quiz");
            quizTable.Columns.Add(new SqlTableColumn("Id") { DataType = new SqlDataType { SqlTypeName = "INT" }, IsNullable = false });
            quizTable.Columns.Add(new SqlTableColumn("Name") { DataType = new SqlDataType { SqlTypeName = "VARCHAR", MaxLength = 50 }, IsNullable = true });
            quizTable.Columns.Add(new SqlTableColumn("CreatedDate") { DataType = new SqlDataType { SqlTypeName = "DATE" }, IsNullable = true });
            quizTable.Columns.Add(new SqlTableColumn("IsActive") { DataType = new SqlDataType { SqlTypeName = "BIT" }, IsNullable = true });
            quizTable.Columns.Add(new SqlTableColumn("PassingScore") { DataType = new SqlDataType { SqlTypeName = "NUMERIC", Precision = 9, Scale = 2 }, IsNullable = true });

            database.Tables.Add(new SqlTable("Question") { Database = database, Schema = new SqlSchemaName("Quizzes") });
            var questionTable = database.Tables.Get("Quizzes", "Question");
            questionTable.Columns.Add(new SqlTableColumn("Id") { DataType = new SqlDataType { SqlTypeName = "INT" }, IsNullable = false });
            questionTable.Columns.Add(new SqlTableColumn("Prompt") { DataType = new SqlDataType { SqlTypeName = "VARCHAR", MaxLength = 100 }, IsNullable = true });
            questionTable.Columns.Add(new SqlTableColumn("CorrectChoice") { DataType = new SqlDataType { SqlTypeName = "CHAR", MaxLength = 1 }, IsNullable = false });

            var model = new CodeGenerationModel();

            // Act
            model.LoadFromSqlDatabase(database);

            // Assert
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
            Assert.Equal(5, quizEntity.Properties.Count);
            Assert.Equal(3, questionEntity.Properties.Count);

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
                x.PropertyType.LogicalType == ModelPropertyLogicalType.Date &&
                !x.IsRequired);

            Assert.Contains(quizEntity.Properties, x =>
                x.Name.Equals("IsActive", StringComparison.Ordinal) &&
                x.PropertyType.LogicalType == ModelPropertyLogicalType.Boolean &&
                !x.IsRequired);

            Assert.Contains(quizEntity.Properties, x =>
                x.Name.Equals("PassingScore", StringComparison.Ordinal) &&
                x.PropertyType.LogicalType == ModelPropertyLogicalType.Decimal &&
                !x.IsRequired);

            Assert.Contains(questionEntity.Properties, x =>
                x.Name.Equals("CorrectChoice", StringComparison.Ordinal) &&
                x.PropertyType.LogicalType == ModelPropertyLogicalType.Character &&
                x.IsRequired);
        }
    }
}
