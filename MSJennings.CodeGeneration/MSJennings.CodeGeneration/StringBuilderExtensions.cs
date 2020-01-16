using System;
using System.Text;

namespace MSJennings.CodeGeneration
{
    public static class StringBuilderExtensions
    {
        public static bool EndsWithNewLine(this StringBuilder sb)
        {
            if (sb == null)
            {
                throw new ArgumentNullException(nameof(sb));
            }

            if (sb.Length < 1)
            {
                return false;
            }

            var n = sb.Length >= 2 ? 2 : 1;
            var ending = sb.ToString(sb.Length - n, n);

            return ending.Equals(Environment.NewLine, StringComparison.OrdinalIgnoreCase) || ending.EndsWith("\n", StringComparison.OrdinalIgnoreCase);
        }
    }
}
