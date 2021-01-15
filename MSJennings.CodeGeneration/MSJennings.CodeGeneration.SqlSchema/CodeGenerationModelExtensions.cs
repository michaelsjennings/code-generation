using MSJennings.CodeGeneration.SqlSchema;
using MSJennings.SqlSchema;
using System;
using System.Threading.Tasks;

namespace MSJennings.CodeGeneration
{
    public static class CodeGenerationModelExtensions
    {
        public static async Task LoadFromSqlDatabase(this CodeGenerationModel model, string connectionString)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            model.Reset();

            var metadata = new SqlMetadata();
            await metadata.LoadFromDatabaseAsync(connectionString);

            var database = new SqlDatabase();
            database.LoadFromMetadata(metadata);

            foreach (var table in database.Tables)
            {
                _ = model.SetCurrentNamespace(table.Schema.Name);
                _ = model.AddEntity(table.Name);

                foreach (var column in table.Columns)
                {
                    var modelPropertyType = column.DataType.ToModelPropertyType();

                    if (modelPropertyType.LogicalType == ModelPropertyLogicalType.List)
                    {
                        if (modelPropertyType.ListItemType.LogicalType == ModelPropertyLogicalType.Object)
                        {
                            _ = model.AddListProperty(column.Name, modelPropertyType.ListItemType.ObjectTypeName, !column.IsNullable);
                        }
                        else
                        {
                            _ = model.AddListProperty(column.Name, modelPropertyType.ListItemType.LogicalType, !column.IsNullable);
                        }
                    }
                    else if (modelPropertyType.LogicalType == ModelPropertyLogicalType.Object)
                    {
                        _ = model.AddProperty(column.Name, column.Name, !column.IsNullable);
                    }
                    else
                    {
                        _ = model.AddProperty(column.Name, modelPropertyType.LogicalType, !column.IsNullable);
                    }
                }
            }
        }
    }
}
