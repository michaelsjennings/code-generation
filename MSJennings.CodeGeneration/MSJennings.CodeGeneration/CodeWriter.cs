using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MSJennings.CodeGeneration
{
    public class CodeWriter
    {
        private readonly StringBuilder _output;

        private readonly IList<FileSegment> _fileSegments;

        private string NoFilesMessage => $"No files have been added to the {GetType().Name}. Use {nameof(BeginFile)}() to add a file.";

        private const string CannotSetIndentMessage = "The indent level can only be increased or decreased when at the start of a new line.";

        private bool IsStartOfNewFile => _output.Length < 1 || _output.Length == _fileSegments.LastOrDefault()?.Start;

        private bool IsStartOfNewLine => _output.Length < 1 || _output.EndsWithNewLine() || IsStartOfNewFile;

        private string Indent => string.Concat(Enumerable.Repeat(IndentString, IndentLevel));

        public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;

        public string FileName { get; private set; }

        public string IndentString { get; set; } = new string(' ', 4);

        public int IndentLevel { get; private set; }

        public bool RemoveFilesFromOutputAfterWriting { get; set; } = true;

        public CodeWriter() : this(null)
        {
        }

        public CodeWriter(StringBuilder output)
        {
            _output = output ?? new StringBuilder();
            _fileSegments = new List<FileSegment>();
        }

        public CodeWriter SetIndent(int indent)
        {
            if (!IsStartOfNewLine)
            {
                throw new InvalidOperationException(CannotSetIndentMessage);
            }

            IndentLevel = indent > 0 ? indent : 0;

            return this;
        }

        public CodeWriter IncreaseIndent()
        {
            return SetIndent(IndentLevel + 1);
        }

        public CodeWriter DecreaseIndent()
        {
            return SetIndent(IndentLevel - 1);
        }

        public CodeWriter AppendIndent()
        {
            _ = _output.Append(Indent);
            return this;
        }

        public CodeWriter Append(string value)
        {
            if (IsStartOfNewLine)
            {
                _ = AppendIndent();
            }

            _ = _output.Append(value);
            return this;
        }

        public CodeWriter Append(string format, params object[] args)
        {
            var value = string.Format(Culture, format, args);
            return Append(value);
        }

        public CodeWriter AppendForEach<T>(IEnumerable<T> items, Func<T, string> transform)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (transform == null)
            {
                throw new ArgumentNullException(nameof(transform));
            }

            foreach (var item in items)
            {
                var value = transform(item);
                _ = Append(value);
            }

            return this;
        }

        public CodeWriter AppendLine()
        {
            _ = _output.AppendLine();

            return this;
        }

        public CodeWriter AppendLine(string value)
        {
            _ = Append(value);

            return AppendLine();
        }

        public CodeWriter AppendLine(string format, params object[] args)
        {
            var value = string.Format(Culture, format, args);
            return AppendLine(value);
        }

        public CodeWriter AppendLineForEach<T>(IEnumerable<T> items, Func<T, string> transform)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (transform == null)
            {
                throw new ArgumentNullException(nameof(transform));
            }

            foreach (var item in items)
            {
                var value = transform(item);

                _ = AppendLine(value);
            }

            return this;
        }

        public CodeWriter ForEach<T>(IEnumerable<T> items, Action<T> action)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            foreach (var item in items)
            {
                action(item);
            }

            return this;
        }

        public CodeWriter BeginFile(string fileName)
        {
            _ = EndFile();

            _fileSegments.Add(
                new FileSegment
                {
                    FileName = fileName,
                    Start = _output.Length
                });

            return this;
        }

        public CodeWriter EndFile()
        {
            var lastSegment = _fileSegments.LastOrDefault();
            if (lastSegment != null)
            {
                if (!lastSegment.Length.HasValue)
                {
                    lastSegment.Length = _output.Length - lastSegment.Start;
                }
            }

            IndentLevel = 0;

            return this;
        }

        public void WriteAllFiles()
        {
            if (_fileSegments.Count < 1)
            {
                throw new InvalidOperationException(NoFilesMessage);
            }

            _ = EndFile();

            for (var i = _fileSegments.Count - 1; i >= 0; i--)
            {
                var fileSegment = _fileSegments[i];
                WriteFileSegment(fileSegment);
            }
        }

        public async Task WriteAllFilesAsync()
        {
            if (_fileSegments.Count < 1)
            {
                throw new InvalidOperationException(NoFilesMessage);
            }

            _ = EndFile();

            for (var i = _fileSegments.Count - 1; i >= 0; i--)
            {
                var fileSegment = _fileSegments[i];
                await WriteFileSegmentAsync(fileSegment);
            }
        }

        public void WriteOneFile(string fileName)
        {
            var fileSegment = GetFileSegment(fileName);
            WriteFileSegment(fileSegment);
        }

        public async Task WriteOneFileAsync(string fileName)
        {
            var fileSegment = GetFileSegment(fileName);
            await WriteFileSegmentAsync(fileSegment);
        }

        public void WriteCurrentFile()
        {
            var lastSegment = _fileSegments.LastOrDefault();
            if (lastSegment == null)
            {
                throw new InvalidOperationException(NoFilesMessage);
            }

            WriteFileSegment(lastSegment);
        }

        public async Task WriteCurrentFileAsync()
        {
            var lastSegment = _fileSegments.LastOrDefault();
            if (lastSegment == null)
            {
                throw new InvalidOperationException(NoFilesMessage);
            }

            await WriteFileSegmentAsync(lastSegment);
        }

        public override string ToString()
        {
            return ToStringAllFiles();
        }

        public string ToString(string fileName)
        {
            return GetFileSegmentContents(fileName);
        }

        public string ToStringCurrentFile()
        {
            var lastSegment = _fileSegments.LastOrDefault();

            return lastSegment != null ? GetFileSegmentContents(lastSegment) : ToStringAllFiles();
        }

        public string ToStringAllFiles()
        {
            return _output.ToString();
        }

        public IDictionary<string, string> ToDictionaryAllFiles()
        {
            var dictionary = new Dictionary<string, string>();

            if (!_fileSegments.Any())
            {
                dictionary.Add("File1", ToStringAllFiles());
            }
            else
            {
                foreach (var fileSegment in _fileSegments)
                {
                    var contents = GetFileSegmentContents(fileSegment);
                    dictionary.Add(fileSegment.FileName, contents);
                }
            }

            return dictionary;
        }

        public void Clear()
        {
            _ = _output.Clear();
            _fileSegments.Clear();
            IndentLevel = 0;
        }

        private FileSegment GetFileSegment(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            var fileSegment = _fileSegments.SingleOrDefault(x => x.FileName.Equals(fileName, StringComparison.OrdinalIgnoreCase));
            if (fileSegment == null)
            {
                throw new ArgumentOutOfRangeException(nameof(fileName));
            }

            return fileSegment;
        }

        private void RemoveFileSegment(FileSegment fileSegment)
        {
            if (fileSegment == null)
            {
                throw new ArgumentNullException(nameof(fileSegment));
            }

            if (!_fileSegments.Contains(fileSegment))
            {
                throw new ArgumentOutOfRangeException(nameof(fileSegment));
            }

            var length = fileSegment.Length ?? _output.Length;
            _ = _output.Remove(fileSegment.Start, length);
            _ = _fileSegments.Remove(fileSegment);

            // decrease each subsequent segment's start location by the length of the segment that was removed
            foreach (var subsequentFileSegment in _fileSegments.Where(x => x.Start > fileSegment.Start))
            {
                subsequentFileSegment.Start -= length;
            }
        }

        private string GetFileSegmentContents(FileSegment fileSegment)
        {
            if (fileSegment == null)
            {
                throw new ArgumentNullException(nameof(fileSegment));
            }

            if (!_fileSegments.Contains(fileSegment))
            {
                throw new ArgumentOutOfRangeException(nameof(fileSegment));
            }

            var length = fileSegment.Length ?? _output.Length;
            return _output.ToString(fileSegment.Start, length);
        }

        private string GetFileSegmentContents(string fileName)
        {
            var fileSegment = GetFileSegment(fileName);
            return GetFileSegmentContents(fileSegment);
        }

        private void WriteFileSegment(FileSegment fileSegment)
        {
            var directoryName = ToFullPath(Path.GetDirectoryName(fileSegment.FileName));
            _ = Directory.CreateDirectory(directoryName);

            var fullFileName = Path.Combine(directoryName, Path.GetFileName(fileSegment.FileName));

            var contents = GetFileSegmentContents(fileSegment);
            File.WriteAllText(fullFileName, contents);

            if (RemoveFilesFromOutputAfterWriting)
            {
                RemoveFileSegment(fileSegment);
            }
        }

        private async Task WriteFileSegmentAsync(FileSegment fileSegment)
        {
            var directoryName = ToFullPath(Path.GetDirectoryName(fileSegment.FileName));
            _ = Directory.CreateDirectory(directoryName);

            var fullFileName = Path.Combine(directoryName, Path.GetFileName(fileSegment.FileName));

            var contents = GetFileSegmentContents(fileSegment);

            // File.WriteAllTextAsync is not supported (yet?) in .Net Standard
            // see: http://jonesie.kiwi/2018/05/17/lemony-snippet-async-file-read-write-in-net-standard-2/
            // await File.WriteAllTextAsync(fileSegment.FileName, contents);

            using (var streamWriter = File.CreateText(fullFileName))
            {
                await streamWriter.WriteAsync(contents);
            }

            if (RemoveFilesFromOutputAfterWriting)
            {
                RemoveFileSegment(fileSegment);
            }
        }

        private static bool IsFullPath(string path)
        {
            return !string.IsNullOrWhiteSpace(path)
                && path.IndexOfAny(Path.GetInvalidPathChars().ToArray()) == -1
                && Path.IsPathRooted(path)
                && !Path.GetPathRoot(path).Equals(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal);
        }

        private static string ToFullPath(string path)
        {
            if (!IsFullPath(path))
            {
                while (path.StartsWith("\\"))
                {
                    path = path.Substring(1);
                }

                var baseDirectoryName = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                path = Path.Combine(baseDirectoryName, path);
            }

            return Path.GetFullPath(path);
        }

        private class FileSegment
        {
            public string FileName { get; set; }

            public int Start { get; set; }

            public int? Length { get; set; }
        }
    }
}
