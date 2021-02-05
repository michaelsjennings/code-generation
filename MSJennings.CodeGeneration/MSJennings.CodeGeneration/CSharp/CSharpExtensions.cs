﻿using System;

namespace MSJennings.CodeGeneration.CSharp
{
    public static class CSharpExtensions
    {
        public static string ToCSharpPublicSingularName(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                throw new ArgumentNullException(nameof(s));
            }

            return s.ToPascalCase().ToSingular();
        }

        public static string ToCSharpPublicPluralName(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                throw new ArgumentNullException(nameof(s));
            }

            return s.ToPascalCase().ToPlural();
        }

        public static string ToCSharpPrivateSingularName(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                throw new ArgumentNullException(nameof(s));
            }

            return s.ToCamelCase().ToSingular();
        }

        public static string ToCSharpPrivatePluralName(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                throw new ArgumentNullException(nameof(s));
            }

            return s.ToCamelCase().ToPlural();
        }

        public static string ToCSharpTypeName(this ModelPropertyType modelPropertyType)
        {
            if (modelPropertyType == null)
            {
                throw new ArgumentNullException(nameof(modelPropertyType));
            }

            if (!string.IsNullOrWhiteSpace(modelPropertyType.ObjectTypeName))
            {
                return modelPropertyType.ObjectTypeName;
            }

            if (modelPropertyType.ListItemType != null)
            {
                if (!string.IsNullOrWhiteSpace(modelPropertyType.ListItemType.ObjectTypeName))
                {
                    return $"IList<{modelPropertyType.ListItemType.ObjectTypeName}>";
                }

                if (modelPropertyType.ListItemType.LogicalType == ModelPropertyLogicalType.KeyValuePair)
                {
                    return "IDictionary<TKey, TValue>";
                }

                return $"IList<${modelPropertyType.ListItemType.LogicalType.ToCSharpTypeName()}>";
            }

            return modelPropertyType.LogicalType.ToCSharpTypeName();
        }

        public static string ToCSharpTypeName(this ModelPropertyLogicalType modelPropertyLogicalType)
        {
            switch (modelPropertyLogicalType)
            {
                case ModelPropertyLogicalType.Boolean:
                    return "bool";

                case ModelPropertyLogicalType.Byte:
                    return "byte";

                case ModelPropertyLogicalType.Date:
                case ModelPropertyLogicalType.DateAndTime:
                    return "DateTime";

                case ModelPropertyLogicalType.Character:
                    return "char";

                case ModelPropertyLogicalType.Decimal:
                    return "decimal";

                case ModelPropertyLogicalType.Integer:
                    return "int";

                case ModelPropertyLogicalType.List:
                    return "IList<T>";

                case ModelPropertyLogicalType.KeyValuePair:
                    return "IKeyValuePair<TKey, TValue>";

                case ModelPropertyLogicalType.String:
                    return "string";

                case ModelPropertyLogicalType.Time:
                    return "TimeSpan";

                case ModelPropertyLogicalType.Object:
                case ModelPropertyLogicalType.Unknown:
                default:
                    return "object";
            }
        }
    }
}
