namespace MSJennings.CodeGeneration
{
    public class CodeGenerationContext
    {
        public CodeWriter CodeWriter { get; private set; }

        public CodeGenerationModel Model { get; private set; }

        public CodeGenerationContext() : this(null, null)
        {
        }

        public CodeGenerationContext(CodeGenerationModel model, CodeWriter codeWriter)
        {
            CodeWriter = codeWriter ?? new CodeWriter();
            Model = model ?? new CodeGenerationModel();
        }
    }
}
