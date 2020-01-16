using MSJennings.SqlSchema;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MSJennings.CodeGeneration
{
    public static class SqlSchemaExtensions
    {
        public static bool IsBoolean(this SqlField field)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            return field.DataType.SqlDbType.IsIn(
                SqlDbType.Bit);
        }

        public static bool IsDate(this SqlField field)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            return field.DataType.SqlDbType.IsIn(
                SqlDbType.Date,
                SqlDbType.DateTime,
                SqlDbType.DateTime2,
                SqlDbType.DateTimeOffset,
                SqlDbType.SmallDateTime);
        }

        public static bool IsInteger(this SqlField field)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            return field.DataType.SqlDbType.IsIn(
                SqlDbType.BigInt,
                SqlDbType.Int,
                SqlDbType.SmallInt,
                SqlDbType.TinyInt);
        }

        public static bool IsNumber(this SqlField field)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            return field.DataType.SqlDbType.IsIn(
                SqlDbType.BigInt,
                SqlDbType.Decimal,
                SqlDbType.Float,
                SqlDbType.Int,
                SqlDbType.Money,
                SqlDbType.Real,
                SqlDbType.SmallInt,
                SqlDbType.SmallMoney,
                SqlDbType.TinyInt);
        }

        public static bool IsString(this SqlField field)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            return field.DataType.SqlDbType.IsIn(
                SqlDbType.Char,
                SqlDbType.NChar,
                SqlDbType.NText,
                SqlDbType.NVarChar,
                SqlDbType.Text,
                SqlDbType.VarChar,
                SqlDbType.Xml);
        }

        public static string CSharpPublicSingularName(this SqlObject value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return value.Name.ToPascalCase().ToSingular();
        }

        public static string CSharpPublicPluralName(this SqlObject value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return value.Name.ToPascalCase().ToPlural();
        }

        public static string CSharpPrivateSingularName(this SqlObject value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return value.Name.ToCamelCase().ToSingular();
        }

        public static string CSharpPrivatePluralName(this SqlObject value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return value.Name.ToCamelCase().ToPlural();
        }

        public static string CSharpTypeName(this SqlColumnBase column, bool includeNullableIfNeeded = true)
        {
            if (column == null)
            {
                throw new ArgumentNullException(nameof(column));
            }

            switch (column.DataType.SqlDbType)
            {
                case SqlDbType.BigInt:
                    return "long" + (includeNullableIfNeeded && column.IsNullable ? "?" : "");

                case SqlDbType.Binary:
                    return "byte[]";

                case SqlDbType.Bit:
                    return "bool" + (includeNullableIfNeeded && column.IsNullable ? "?" : "");

                case SqlDbType.Char:
                    return "char";

                case SqlDbType.Date:
                    return "DateTime" + (includeNullableIfNeeded && column.IsNullable ? "?" : "");

                case SqlDbType.DateTime:
                    return "DateTime" + (includeNullableIfNeeded && column.IsNullable ? "?" : "");

                case SqlDbType.DateTime2:
                    return "DateTime" + (includeNullableIfNeeded && column.IsNullable ? "?" : "");

                case SqlDbType.DateTimeOffset:
                    return "DateTimeOffset" + (includeNullableIfNeeded && column.IsNullable ? "?" : "");

                case SqlDbType.Decimal:
                    return "decimal" + (includeNullableIfNeeded && column.IsNullable ? "?" : "");

                case SqlDbType.Float:
                    return "double" + (includeNullableIfNeeded && column.IsNullable ? "?" : "");

                case SqlDbType.Image:
                    return "byte[]";

                case SqlDbType.Int:
                    return "int" + (includeNullableIfNeeded && column.IsNullable ? "?" : "");

                case SqlDbType.Money:
                    return "decimal" + (includeNullableIfNeeded && column.IsNullable ? "?" : "");

                case SqlDbType.NChar:
                    return "string";

                case SqlDbType.NText:
                    return "string";

                case SqlDbType.NVarChar:
                    return "string";

                case SqlDbType.Real:
                    return "float" + (includeNullableIfNeeded && column.IsNullable ? "?" : "");

                case SqlDbType.SmallDateTime:
                    return "DateTime" + (includeNullableIfNeeded && column.IsNullable ? "?" : "");

                case SqlDbType.SmallInt:
                    return "short" + (includeNullableIfNeeded && column.IsNullable ? "?" : "");

                case SqlDbType.SmallMoney:
                    return "decimal" + (includeNullableIfNeeded && column.IsNullable ? "?" : "");

                case SqlDbType.Structured:
                    return "DataTable";

                case SqlDbType.Text:
                    return "string";

                case SqlDbType.Time:
                    return "DateTime" + (includeNullableIfNeeded && column.IsNullable ? "?" : "");

                case SqlDbType.Timestamp:
                    return "byte[]";

                case SqlDbType.TinyInt:
                    return "byte";

                case SqlDbType.Udt:
                    return "object";

                case SqlDbType.UniqueIdentifier:
                    return "Guid" + (includeNullableIfNeeded && column.IsNullable ? "?" : "");

                case SqlDbType.VarBinary:
                    return "byte[]";

                case SqlDbType.VarChar:
                    return "string";

                case SqlDbType.Variant:
                    return "object";

                case SqlDbType.Xml:
                    return "string";

                default:
                    return "object";
            }
        }

        public static IEnumerable<SqlColumnBase> ReferenceIdColumns(this SqlTableViewBase tableOrView)
        {
            if (tableOrView == null)
            {
                throw new ArgumentNullException(nameof(tableOrView));
            }

            var otherTablesAndViews = tableOrView.Database.TablesAndViews.ToList();

            var idColumns = tableOrView.Columns.Where(x =>
                !x.Name.Equals("Id", StringComparison.OrdinalIgnoreCase) &&
                x.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase));

            foreach (var idColumn in idColumns)
            {
                var nameWithoutId = idColumn.Name.Remove(idColumn.Name.Length - 2);
                var nameWithoutIdSingular = nameWithoutId.ToSingular();
                var nameWithoutIdPlural = nameWithoutId.ToPlural();

                if (otherTablesAndViews.Any(x =>
                        nameWithoutIdSingular.EndsWith(x.CSharpPublicSingularName(), StringComparison.OrdinalIgnoreCase) ||
                        nameWithoutIdPlural.EndsWith(x.CSharpPublicSingularName(), StringComparison.OrdinalIgnoreCase)))
                {
                    yield return idColumn;
                }
            }
        }
    }
}
