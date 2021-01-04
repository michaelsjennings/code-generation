using System.Collections.Generic;

namespace MSJennings.CodeGeneration.Tests.TestAssembly.Quizzes
{
    public class Question
    {
        public int Id { get; set; }

        public string Prompt { get; set; }

        public IDictionary<char, string> Choices { get; } = new Dictionary<char, string>();

        public char CorrectChoice { get; set; }
    }
}
