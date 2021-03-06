﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace MSJennings.CodeGeneration
{
    public static class ReflectionExtensions
    {
        public static ModelPropertyType ToModelPropertyType(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var logicalType = type.ToModelPropertyLogicalType();

            if (logicalType == ModelPropertyLogicalType.List)
            {
                var isDictionary =
                    (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>)) ||
                    type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDictionary<,>));

                if (isDictionary)
                {
                    return new ModelPropertyType
                    {
                        LogicalType = logicalType,
                        ObjectTypeName = null,
                        ListItemType = new ModelPropertyType
                        {
                            LogicalType = ModelPropertyLogicalType.KeyValuePair,
                            ObjectTypeName = null,
                            ListItemType = null,
                        }
                    };
                }
                else
                {
                    var listItemType = type.GetGenericArguments().FirstOrDefault() ?? type.GetElementType();

                    return new ModelPropertyType
                    {
                        LogicalType = logicalType,
                        ObjectTypeName = null,
                        ListItemType = listItemType.ToModelPropertyType(),
                    };
                }
            }

            if (logicalType == ModelPropertyLogicalType.Object)
            {
                if (type.IsGenericType)
                {
                    return type.ToGenericModelPropertyType();
                }

                return new ModelPropertyType
                {
                    LogicalType = logicalType,
                    ObjectTypeName = type.Name,
                    ListItemType = null,
                };
            }

            return new ModelPropertyType
            {
                LogicalType = logicalType,
                ObjectTypeName = null,
                ListItemType = null,
            };
        }

        private static ModelPropertyLogicalType ToModelPropertyLogicalType(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            else if (type == typeof(bool))
            {
                return ModelPropertyLogicalType.Boolean;
            }
            else if (type.IsIn(typeof(byte), typeof(sbyte)))
            {
                return ModelPropertyLogicalType.Byte;
            }
            else if (type == typeof(char))
            {
                return ModelPropertyLogicalType.Character;
            }
            else if (type == typeof(DateTime))
            {
                return ModelPropertyLogicalType.DateAndTime;
            }
            else if (type.IsIn(typeof(float), typeof(double), typeof(decimal)))
            {
                return ModelPropertyLogicalType.Decimal;
            }
            else if (type.IsIn(typeof(short), typeof(int), typeof(long), typeof(ushort), typeof(uint), typeof(ulong)))
            {
                return ModelPropertyLogicalType.Integer;
            }
            else if (type.IsIn(typeof(string), typeof(Guid)))
            {
                return ModelPropertyLogicalType.String;
            }
            else if (type == typeof(TimeSpan))
            {
                return ModelPropertyLogicalType.Time;
            }
            else if (type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(KeyValuePair<,>)))
            {
                return ModelPropertyLogicalType.KeyValuePair;
            }
            else if (type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            {
                return ModelPropertyLogicalType.List;
            }
            else
            {
                return ModelPropertyLogicalType.Object;
            }
        }

        private static ModelPropertyType ToGenericModelPropertyType(this Type type)
        {
            if (!type.IsGenericType)
            {
                return type.ToModelPropertyType();
            }

            var modelPropertyType = new ModelPropertyType
            {
                LogicalType = ModelPropertyLogicalType.Object,
                ObjectTypeName = type.Name.Substring(0, type.Name.IndexOf("`", StringComparison.Ordinal))
            };

            foreach (var argumentType in type.GetGenericArguments())
            {
                modelPropertyType.GenericArgumentTypes.Add(argumentType.ToModelPropertyType());
            }

            return modelPropertyType;
        }

        public static bool HasRequiredAttribute(this PropertyInfo property)
        {
            return Attribute.IsDefined(property, typeof(RequiredAttribute));
        }
    }
}
