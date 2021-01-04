using System;
using System.Collections.Generic;

namespace MSJennings.CodeGeneration.Tests.TestAssembly.Quizzes
{
    public class Quiz
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsActive { get; set; }

        public decimal PassingScore { get; set; }

        public IList<string> Topics { get; } = new List<string>();

        public IList<Question> Questions { get; } = new List<Question>();
    }
}
