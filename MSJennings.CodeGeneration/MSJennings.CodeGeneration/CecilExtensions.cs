using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MSJennings.CodeGeneration
{
    internal static class CecilExtensions
    {
        internal static ModelPropertyLogicalType ToModelPropertyLogicalType(this TypeReference typeReference)
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
            else if (typeReference.IsGenericInstance && typeReference.Resolve().Interfaces.Any(x => x.InterfaceType.FullName.StartsWith(typeof(IEnumerable<>).FullName)))
            {
                return ModelPropertyLogicalType.List;
            }
            else
            {
                return ModelPropertyLogicalType.Object;
            }
        }
    }
}
