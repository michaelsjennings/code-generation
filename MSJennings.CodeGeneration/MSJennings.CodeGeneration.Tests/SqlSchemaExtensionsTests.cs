using MSJennings.SqlSchema;
using MSJennings.CodeGeneration.SqlSchema;
using System;
using System.Linq;
using Xunit;

namespace MSJennings.CodeGeneration.Tests
{
    public class SqlSchemaExtensionsTests
    {
        [Fact]
        public void ReferenceIdColumns_WithSingularTableNames_ShouldReturnExpectedReferenceIdColumns()
        {
            // Arrange
            var database = new SqlDatabase();
            database.Tables.Add(new SqlTable("Project") { Database = database, Schema = new SqlSchemaName("dbo") });
            database.Tables.Add(new SqlTable("Issue") { Database = database, Schema = new SqlSchemaName("dbo") });
            database.Tables.Add(new SqlTable("Note") { Database = database, Schema = new SqlSchemaName("dbo") });
            database.Tables.Add(new SqlTable("Attachment") { Database = database, Schema = new SqlSchemaName("dbo") });
            database.Views.Add(new SqlView("User") { Database = database, Schema = new SqlSchemaName("dbo") });

            var issueTable = database.Tables.Get("dbo", "Issue");
            issueTable.Columns.Add(new SqlTableColumn("Id"));
            issueTable.Columns.Add(new SqlTableColumn("Title"));
            issueTable.Columns.Add(new SqlTableColumn("ProjectId"));
            issueTable.Columns.Add(new SqlTableColumn("CreatedByUserId"));
            issueTable.Columns.Add(new SqlTableColumn("CreatedDate"));
            issueTable.Columns.Add(new SqlTableColumn("OtherId"));

            // Act
            var result = issueTable.ReferenceIdColumns().ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, x => x.Name.Equals("ProjectId", StringComparison.Ordinal));
            Assert.Contains(result, x => x.Name.Equals("CreatedByUserId", StringComparison.Ordinal));
        }

        [Fact]
        public void ReferenceIdColumns_WithPluralTableNames_ShouldReturnExpectedReferenceIdColumns()
        {
            // Arrange
            var database = new SqlDatabase();
            database.Tables.Add(new SqlTable("Projects") { Database = database, Schema = new SqlSchemaName("dbo") });
            database.Tables.Add(new SqlTable("Issues") { Database = database, Schema = new SqlSchemaName("dbo") });
            database.Tables.Add(new SqlTable("Notes") { Database = database, Schema = new SqlSchemaName("dbo") });
            database.Tables.Add(new SqlTable("Attachments") { Database = database, Schema = new SqlSchemaName("dbo") });
            database.Views.Add(new SqlView("Users") { Database = database, Schema = new SqlSchemaName("dbo") });

            var issueTable = database.Tables.Get("dbo", "Issues");
            issueTable.Columns.Add(new SqlTableColumn("Id"));
            issueTable.Columns.Add(new SqlTableColumn("Title"));
            issueTable.Columns.Add(new SqlTableColumn("ProjectId"));
            issueTable.Columns.Add(new SqlTableColumn("CreatedByUserId"));
            issueTable.Columns.Add(new SqlTableColumn("CreatedDate"));
            issueTable.Columns.Add(new SqlTableColumn("OtherId"));

            // Act
            var result = issueTable.ReferenceIdColumns().ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, x => x.Name.Equals("ProjectId", StringComparison.Ordinal));
            Assert.Contains(result, x => x.Name.Equals("CreatedByUserId", StringComparison.Ordinal));
        }
    }
}
