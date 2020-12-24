using System.Collections.Generic;

namespace MSJennings.CodeGeneration
{
    public class ModelNamespace
    {
        public CodeGenerationModel Model { get; set; }

        public string Name { get; set; }

        public IList<ModelEntity> Entities { get; } = new List<ModelEntity>();
    }
}
