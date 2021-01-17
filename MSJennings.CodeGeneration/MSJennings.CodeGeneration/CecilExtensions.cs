using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MSJennings.CodeGeneration
{
    public static class CecilExtensions
    {
#pragma warning disable CS3001 // Argument type is not CLS-compliant
#pragma warning disable CS3002 // Return type is not CLS-compliant
        public static ModelPropertyType ToModelPropertyType(this TypeReference typeReference)
        {
            if (typeReference == null)
            {
                throw new ArgumentNullException(nameof(typeReference));
            }

            var logicalType = typeReference.ToModelPropertyLogicalType();
            if (logicalType == ModelPropertyLogicalType.List)
            {
                var isDictionary =
                    typeReference.FullName.StartsWith(typeof(IDictionary<,>).FullName, StringComparison.Ordinal) ||
                    typeReference.Resolve().Interfaces.Any(x => x.InterfaceType.FullName.StartsWith(typeof(IDictionary<,>).FullName, StringComparison.Ordinal));

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
                else if (typeReference.IsArray)
                {
                    return new ModelPropertyType
                    {
                        LogicalType = logicalType,
                        ObjectTypeName = null,
                        ListItemType = typeReference.GetElementType().ToModelPropertyType(),
                    };
                }
                else
                {
                    var genericInstanceType = (GenericInstanceType)typeReference;
                    var listItemType = genericInstanceType.GenericArguments.FirstOrDefault();

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
                return new ModelPropertyType
                {
                    LogicalType = logicalType,
                    ObjectTypeName = typeReference.Name,
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

        private static ModelPropertyLogicalType ToModelPropertyLogicalType(this TypeReference typeReference)
        {
            if (typeReference == null)
            {
                throw new ArgumentNullException(nameof(typeReference));
            }
            else if (typeReference.FullName.Equals(typeof(bool).FullName, StringComparison.Ordinal))
            {
                return ModelPropertyLogicalType.Boolean;
            }
            else if (typeReference.FullName.IsIn(StringComparison.Ordinal, typeof(byte).FullName, typeof(sbyte).FullName))
            {
                return ModelPropertyLogicalType.Boolean;
            }
            else if (typeReference.FullName.Equals(typeof(char).FullName, StringComparison.Ordinal))
            {
                return ModelPropertyLogicalType.Character;
            }
            else if (typeReference.FullName.Equals(typeof(DateTime).FullName, StringComparison.Ordinal))
            {
                return ModelPropertyLogicalType.DateAndTime;
            }
            else if (typeReference.FullName.IsIn(StringComparison.Ordinal, typeof(float).FullName, typeof(double).FullName, typeof(decimal).FullName))
            {
                return ModelPropertyLogicalType.Decimal;
            }
            else if (typeReference.FullName.IsIn(StringComparison.Ordinal, typeof(short).FullName, typeof(int).FullName, typeof(long).FullName, typeof(ushort).FullName, typeof(uint).FullName, typeof(ulong).FullName))
            {
                return ModelPropertyLogicalType.Integer;
            }
            else if (typeReference.FullName.IsIn(StringComparison.Ordinal, typeof(string).FullName, typeof(Guid).FullName))
            {
                return ModelPropertyLogicalType.String;
            }
            else if (typeReference.FullName.Equals(typeof(TimeSpan).FullName, StringComparison.Ordinal))
            {
                return ModelPropertyLogicalType.Time;
            }
            else if (typeReference.IsGenericInstance && typeReference.Resolve().Interfaces.Any(x => x.InterfaceType.FullName.StartsWith(typeof(KeyValuePair<,>).FullName)))
            {
                return ModelPropertyLogicalType.KeyValuePair;
            }
            else if (typeReference.IsArray || (typeReference.IsGenericInstance && typeReference.Resolve().Interfaces.Any(x => x.InterfaceType.FullName.StartsWith(typeof(IEnumerable<>).FullName))))
            {
                return ModelPropertyLogicalType.List;
            }
            else
            {
                return ModelPropertyLogicalType.Object;
            }
        }

        public static IEnumerable<PropertyDefinition> GetPublicInstanceProperties(this TypeDefinition typeDefinition)

        {
            if (typeDefinition == null)
            {
                throw new ArgumentNullException(nameof(typeDefinition));
            }

            return typeDefinition.Properties.Where(x => x.HasThis && (x.PropertyType.Resolve()?.IsPublic ?? false));
        }

        public static bool HasRequiredAttribute(this PropertyDefinition propertyDefinition)
        {
            if (propertyDefinition == null)
            {
                throw new ArgumentNullException(nameof(propertyDefinition));
            }

            return propertyDefinition.CustomAttributes.Any(x => x.AttributeType.FullName.Equals(typeof(RequiredAttribute).FullName, StringComparison.Ordinal));
        }
#pragma warning restore CS3001 // Argument type is not CLS-compliant
#pragma warning restore CS3002 // Return type is not CLS-compliant
    }
}
