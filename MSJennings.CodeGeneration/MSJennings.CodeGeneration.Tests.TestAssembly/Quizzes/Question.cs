using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MSJennings.CodeGeneration.Tests.TestAssembly.Quizzes
{
    public class Question
    {
        public int Id { get; set; }

        public string Prompt { get; set; }

        [Required]
        public IDictionary<char, string> Choices { get; } = new Dictionary<char, string>();

        public char CorrectChoice { get; set; }

        public int[] QuizIds { get; set; }
    }
}
