using MSJennings.SqlSchema;
using System;
using System.Text;
using Xunit;

namespace MSJennings.CodeGeneration.SqlSchema.Tests
{
    public class CodeWriterTests
    {
        [Fact]
        public void ToString_WithSqlTable_ShouldReturnExpectedResult()
        {
            // arrange
            var table = new SqlTable("Employee");
            table.Columns.Add(new SqlTableColumn("Id") { Table = table, DataType = new SqlDataType { SqlTypeName = "INT" }, IsIdentity = true });
            table.Columns.Add(new SqlTableColumn("FirstName") { Table = table, DataType = new SqlDataType { SqlTypeName = "VARCHAR", MaxLength = 50 } });
            table.Columns.Add(new SqlTableColumn("LastName") { Table = table, DataType = new SqlDataType { SqlTypeName = "VARCHAR", MaxLength = 50 } });
            table.Columns.Add(new SqlTableColumn("HireDate") { Table = table, DataType = new SqlDataType { SqlTypeName = "DATETIME" } });
            table.Columns.Add(new SqlTableColumn("TerminationDate") { Table = table, DataType = new SqlDataType { SqlTypeName = "DATETIME" }, IsNullable = true });
            table.Columns.Add(new SqlTableColumn("Salary") { Table = table, DataType = new SqlDataType { SqlTypeName = "MONEY" } });
            table.Columns.Add(new SqlTableColumn("HasBenefits") { Table = table, DataType = new SqlDataType { SqlTypeName = "BIT" } });

            var writer = new CodeWriter
            {
                IndentString = "  " // two spaces
            };

            StringExtensions.ToSingularSpecialCases.Add("HasBenefits", "HasBenefits");

            writer
                .AppendLine("using System")
                .AppendLine()
                .AppendLine("namespace CodeWriter.Test")
                .AppendLine("{")
                .IncreaseIndent()
                .AppendLine($"public class {table.Name}")
                .AppendLine("{")
                .IncreaseIndent()
                .AppendLineForEach(table.Columns, x => $"public {x.CSharpTypeName()} {x.CSharpPublicSingularName()} {{ get; set; }}{(!x.IsLastIn(table.Columns) ? Environment.NewLine : "")}")
                .DecreaseIndent()
                .AppendLine("}")
                .DecreaseIndent()
                .AppendLine("}");

            var expected = new StringBuilder()
                .AppendLine("using System")
                .AppendLine()
                .AppendLine("namespace CodeWriter.Test")
                .AppendLine("{")
                .AppendLine("  public class Employee")
                .AppendLine("  {")
                .AppendLine("    public int Id { get; set; }")
                .AppendLine()
                .AppendLine("    public string FirstName { get; set; }")
                .AppendLine()
                .AppendLine("    public string LastName { get; set; }")
                .AppendLine()
                .AppendLine("    public DateTime HireDate { get; set; }")
                .AppendLine()
                .AppendLine("    public DateTime? TerminationDate { get; set; }")
                .AppendLine()
                .AppendLine("    public decimal Salary { get; set; }")
                .AppendLine()
                .AppendLine("    public bool HasBenefits { get; set; }")
                .AppendLine("  }")
                .AppendLine("}")
                .ToString();

            // act
            var result = writer.ToString();

            // assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void WriteAllFiles_WithSqlDatabase_ShouldWriteFiles()
        {
            // arrange
            var connectionString = @"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=AdventureWorks2016; Integrated Security=True;";

            var metadata = new SqlMetadata();
            metadata.LoadFromDatabase(connectionString);

            var database = new SqlDatabase();
            database.LoadFromMetadata(metadata);

            // act

            // assert
        }
    }
}
