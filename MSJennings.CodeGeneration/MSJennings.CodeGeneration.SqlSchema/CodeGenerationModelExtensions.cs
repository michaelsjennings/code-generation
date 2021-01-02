using System;

namespace MSJennings.CodeGeneration
{
    public static class CodeGenerationModelExtensions
    {
        public static void LoadFromSqlDatabase(this CodeGenerationModel model, string connectionString)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            throw new NotImplementedException();
        }
    }
}
