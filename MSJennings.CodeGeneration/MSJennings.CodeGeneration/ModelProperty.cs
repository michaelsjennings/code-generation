namespace MSJennings.CodeGeneration
{
    public class ModelProperty
    {
        public ModelEntity Entity { get; set; }

        public string Name { get; set; }

        public ModelPropertyType PropertyType { get; set; }

        public bool IsRequired { get; set; }
    }
}
