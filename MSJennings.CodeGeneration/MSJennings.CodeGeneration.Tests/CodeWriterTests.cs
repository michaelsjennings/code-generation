using MSJennings.SqlSchema;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MSJennings.CodeGeneration.Tests
{
    public class CodeWriterTests
    {
        [Fact]
        public void ToString_WithStringLiterals_ShouldReturnExpectedResult()
        {
            // arrange
            var writer = new CodeWriter
            {
                IndentString = "  " // two spaces
            };

            writer
                .AppendLine("using System")
                .AppendLine()
                .AppendLine("namespace CodeWriter.Test")
                .AppendLine("{")
                .IncreaseIndent()
                .AppendLine("public class Employee")
                .AppendLine("{")
                .IncreaseIndent()
                .AppendLine("public int Id { get; set; }")
                .AppendLine()
                .AppendLine("public string FirstName { get; set; }")
                .AppendLine()
                .AppendLine("public string LastName { get; set; }")
                .AppendLine()
                .AppendLine("public DateTime HireDate { get; set; }")
                .AppendLine()
                .AppendLine("public DateTime? TerminationDate { get; set; }")
                .AppendLine()
                .AppendLine("public decimal Salary { get; set; }")
                .AppendLine()
                .AppendLine("public bool HasBenefits { get; set; }")
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
                .ToString(); ;

            // act
            var result = writer.ToString();

            // assert
            Assert.Equal(expected, result);
        }

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
        public async Task WriteOneFileAsync_WithValidFileName_ShouldWriteFile()
        {
            // arrange
            var writer = new CodeWriter();

            var fileName = Path.GetTempFileName();
            writer
                .BeginFile(fileName)
                .AppendLine("Line 1")
                .IncreaseIndent()
                .Append("Line ")
                .Append("2")
                .AppendLine()
                .DecreaseIndent()
                .AppendLine("Line 3")
                .EndFile();

            var expected = new StringBuilder()
                .AppendLine("Line 1")
                .AppendLine("    Line 2")
                .AppendLine("Line 3")
                .ToString();

            // act
            await writer.WriteOneFileAsync(fileName);
            var actual = await File.ReadAllTextAsync(fileName);

            // assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task WriteAllFilesAsync_WithMultipleFiles_ShouldWriteFiles()
        {
            // arrange
            var writer = new CodeWriter();

            var fileNames = new[]
            {
                Path.GetTempFileName(),
                Path.GetTempFileName(),
                Path.GetTempFileName(),
                Path.GetTempFileName(),
                Path.GetTempFileName()
            };

            var lines = new[] { 1, 2, 3 };
            

            for (var i = 0; i < fileNames.Length; i++)
            {
                writer
                    .BeginFile(fileNames[i])
                    .AppendLineForEach(lines, x => $"File {i + 1} Line {x}")
                    .EndFile();
            }

            var file2ExpectedContents = new StringBuilder()
                .AppendLine("File 2 Line 1")
                .AppendLine("File 2 Line 2")
                .AppendLine("File 2 Line 3")
                .ToString();

            // act
            await writer.WriteAllFilesAsync();
            var file2ActualContents = await File.ReadAllTextAsync(fileNames[1]);

            // assert
            foreach (var fileName in fileNames)
            {
                Assert.True(new FileInfo(fileName).Length > 0);
            }

            Assert.Equal(file2ExpectedContents, file2ActualContents);
        }

        [Fact]
        public void WriteAllFiles_WithSomeOutputOutsideOfFiles_ShouldWriteOnlyExpectedContentToFiles()
        {
            // arrange
            var writer = new CodeWriter();

            var fileName1 = Path.GetTempFileName();
            var fileName2 = Path.GetTempFileName();

            writer.AppendLine("This should be the first line in the default output file.");

            writer.BeginFile(fileName1);
            writer.AppendLine("This is file 1");
            writer.EndFile();

            writer.AppendLine("This should be the middle line in the default output file.");

            writer.BeginFile(fileName2);
            writer.AppendLine("This is file 2");
            writer.EndFile();

            writer.AppendLine("This should be the last line in the default output file.");

            var file1ExpectedContents = "This is file 1" + Environment.NewLine;
            var file2ExpectedContents = "This is file 2" + Environment.NewLine;
            var defaultOutputExpectedContents = new StringBuilder()
                .AppendLine("This should be the first line in the default output file.")
                .AppendLine("This should be the middle line in the default output file.")
                .AppendLine("This should be the last line in the default output file.")
                .ToString();

            // act
            writer.WriteAllFiles();
            var file1ActualContents = File.ReadAllText(fileName1);
            var file2ActualContents = File.ReadAllText(fileName2);
            var defaultOutputActualContents = writer.ToString();

            // assert
            Assert.Equal(file1ExpectedContents, file1ActualContents);
            Assert.Equal(file2ExpectedContents, file2ActualContents);
            Assert.Equal(defaultOutputExpectedContents, defaultOutputActualContents);
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
