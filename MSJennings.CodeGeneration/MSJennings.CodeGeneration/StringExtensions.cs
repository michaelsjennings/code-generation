using Humanizer;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MSJennings.CodeGeneration
{
    public static class StringExtensions
    {
        public static IDictionary<string, string> ToSingularSpecialCases { get; private set; } = new Dictionary<string, string>(StringComparer.Ordinal);

        public static IDictionary<string, string> ToPluralSpecialCases { get; private set; } = new Dictionary<string, string>(StringComparer.Ordinal);

        public static IEnumerable<string> ToWordsList(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                throw new ArgumentNullException(nameof(s));
            }

            // replace non-alphanumerics with an underscore
            var words = Regex.Replace(s, "\\W", "_");

            // insert an underscore between transitions from lowercase to uppsercase
            // example: "My123FileHTTPPath4" becomes "My123File_HTTPPath4"
            words = Regex.Replace(words, "([a-z])([A-Z])", "$1_$2");

            // insert an underscore between transitions from all-uppercase words to standard case words
            // example: "My123File_HTTPPath4" becomes "My123File_HTTP_Path4"
            words = Regex.Replace(words, "([A-Z]+)([A-Z][a-z])", "$1_$2");

            // insert an underscore between transitions from numbers to letters
            // example: "My123File_HTTP_Path4" becomes "My123_File_HTTP_Path4"
            words = Regex.Replace(words, "(\\d)([A-Za-z])", "$1_$2");

            // insert an underscore between transitions from letters to numbers
            // example: "My123_File_HTTP_Path4" becomes "My_123_File_HTTP_Path_4"
            words = Regex.Replace(words, "([A-Za-z])(\\d)", "$1_$2");

            // replace multiple consecutive occurrences of an underscore with a single underscore
            words = Regex.Replace(words, @"_{2,}", "_");

            // split into words at the underscores
            // example: "My_123_File_HTTP_Path_4" becomes "My", "123", "File", "HTTP", "Path", and "4"
            var wordsList = words.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);

            return wordsList;
        }

        public static string ToWordsString(this string s, string separator = " ")
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                throw new ArgumentNullException(nameof(s));
            }

            var words = s.ToWordsList();
            return string.Join(separator, words);
        }

        public static string ToCamelCase(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                throw new ArgumentNullException(nameof(s));
            }

            var words = s.ToWordsString("_");
            return words.Camelize();
        }

        public static string ToPascalCase(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                throw new ArgumentNullException(nameof(s));
            }

            var words = s.ToWordsString("_");
            return words.Pascalize();
        }

        public static string ToSingular(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                throw new ArgumentNullException(nameof(s));
            }

            return ToSingularSpecialCases.ContainsKey(s)
                ? ToSingularSpecialCases[s]
                : s.Singularize(inputIsKnownToBePlural: false);
        }

        public static string ToPlural(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                throw new ArgumentNullException(nameof(s));
            }

            return ToPluralSpecialCases.ContainsKey(s)
                ? ToPluralSpecialCases[s]
                : s.Pluralize(inputIsKnownToBeSingular: false);
        }

        public static bool EndsWithNewLine(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                throw new ArgumentNullException(nameof(s));
            }

            return s.EndsWith(Environment.NewLine, StringComparison.Ordinal);
        }
    }
}
