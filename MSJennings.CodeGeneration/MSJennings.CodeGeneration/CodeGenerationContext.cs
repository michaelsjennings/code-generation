namespace MSJennings.CodeGeneration
{
    public class CodeGenerationContext
    {
        public CodeWriter CodeWriter { get; private set; }

        public CodeGenerationModel Model { get; private set; }

        public CodeGenerationContext() : this(null)
        {
        }

        public CodeGenerationContext(CodeWriter codeWriter)
        {
            CodeWriter = codeWriter ?? new CodeWriter();
        }
    }
}
