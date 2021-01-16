using System.Collections.Generic;

namespace MSJennings.CodeGeneration
{
    public class ModelEntity
    {
        public ModelNamespace Namespace { get; set; }

        public string Name { get; set; }

        public IList<ModelProperty> Properties { get; } = new List<ModelProperty>();
    }
}
