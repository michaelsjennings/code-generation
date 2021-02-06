using System.Collections.Generic;

namespace MSJennings.CodeGeneration
{
    public class ModelPropertyType
    {
        public ModelPropertyLogicalType LogicalType { get; set; }

        public string ObjectTypeName { get; set; }

        public ModelPropertyType ListItemType { get; set; }

        public IList<ModelPropertyType> GenericArgumentTypes { get; } = new List<ModelPropertyType>();
    }

    public enum ModelPropertyLogicalType
    {
#pragma warning disable CA1720 // Identifier contains type name
        Unknown,
        Object,
        Boolean,
        Byte,
        Date,
        DateAndTime,
        Character,
        Decimal,
        Integer,
        List,
        KeyValuePair,
        String,
        Time,
#pragma warning restore CA1720 // Identifier contains type name
    }
}
