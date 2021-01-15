using MSJennings.CodeGeneration.SqlSchema;
using MSJennings.SqlSchema;
using System;

namespace MSJennings.CodeGeneration
{
    public static class CodeGenerationModelExtensions
    {
        public static void LoadFromSqlDatabase(this CodeGenerationModel model, SqlDatabase database)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (database == null)
            {
                throw new ArgumentNullException(nameof(database));
            }

            model.Reset();

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
