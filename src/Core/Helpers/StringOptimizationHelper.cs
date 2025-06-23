using System.Buffers;
using System.Text;

namespace Core.Helpers
{
    /// <summary>
    /// Optimized string operations to reduce allocations and improve performance
    /// </summary>
    public static class StringOptimizationHelper
    {
        private static readonly ArrayPool<char> CharPool = ArrayPool<char>.Shared;

        /// <summary>
        /// Optimized string concatenation using StringBuilder for multiple strings
        /// </summary>
        /// <param name="strings">Strings to concatenate</param>
        /// <returns>Concatenated string</returns>
        public static string ConcatOptimized(params string[] strings)
        {
            if (strings == null || strings.Length == 0)
                return string.Empty;

            if (strings.Length == 1)
                return strings[0] ?? string.Empty;

            var totalLength = 0;
            for (int i = 0; i < strings.Length; i++)
            {
                totalLength += strings[i]?.Length ?? 0;
            }

            var sb = new StringBuilder(totalLength);
            for (int i = 0; i < strings.Length; i++)
            {
                if (strings[i] != null)
                    sb.Append(strings[i]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Optimized string join using StringBuilder
        /// </summary>
        /// <param name="separator">Separator string</param>
        /// <param name="values">Values to join</param>
        /// <returns>Joined string</returns>
        public static string JoinOptimized(string separator, IEnumerable<string> values)
        {
            if (values == null)
                return string.Empty;

            var valuesList = values.ToList();
            if (valuesList.Count == 0)
                return string.Empty;

            if (valuesList.Count == 1)
                return valuesList[0] ?? string.Empty;

            var separatorLength = separator?.Length ?? 0;
            var totalLength = valuesList.Sum(v => (v?.Length ?? 0)) + (separatorLength * (valuesList.Count - 1));

            var sb = new StringBuilder(totalLength);
            for (int i = 0; i < valuesList.Count; i++)
            {
                if (i > 0 && separator != null)
                    sb.Append(separator);
                
                if (valuesList[i] != null)
                    sb.Append(valuesList[i]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Optimized string trimming and lowercasing in one operation
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>Trimmed and lowercased string</returns>
        public static string TrimAndLowerOptimized(string? input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            var span = input.AsSpan().Trim();
            if (span.IsEmpty)
                return string.Empty;

            // Use ArrayPool for temporary buffer
            var buffer = CharPool.Rent(span.Length);
            try
            {
                for (int i = 0; i < span.Length; i++)
                {
                    buffer[i] = char.ToLowerInvariant(span[i]);
                }

                return new string(buffer, 0, span.Length);
            }
            finally
            {
                CharPool.Return(buffer);
            }
        }

        /// <summary>
        /// Optimized contains check with case-insensitive comparison
        /// </summary>
        /// <param name="source">Source string</param>
        /// <param name="value">Value to search for</param>
        /// <returns>True if contains, false otherwise</returns>
        public static bool ContainsIgnoreCase(string? source, string? value)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(value))
                return false;

            return source.Contains(value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Optimized string replacement for multiple replacements
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="replacements">Dictionary of old->new value pairs</param>
        /// <returns>String with replacements applied</returns>
        public static string ReplaceMultiple(string input, Dictionary<string, string> replacements)
        {
            if (string.IsNullOrEmpty(input) || replacements == null || replacements.Count == 0)
                return input ?? string.Empty;

            var result = input;
            foreach (var replacement in replacements)
            {
                if (!string.IsNullOrEmpty(replacement.Key))
                {
                    result = result.Replace(replacement.Key, replacement.Value ?? string.Empty);
                }
            }

            return result;
        }

        /// <summary>
        /// Optimized string splitting with trimming
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="separator">Separator character</param>
        /// <param name="removeEmptyEntries">Whether to remove empty entries</param>
        /// <returns>Array of trimmed strings</returns>
        public static string[] SplitAndTrim(string? input, char separator, bool removeEmptyEntries = true)
        {
            if (string.IsNullOrEmpty(input))
                return Array.Empty<string>();

            var options = removeEmptyEntries ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None;
            var parts = input.Split(separator, options);

            for (int i = 0; i < parts.Length; i++)
            {
                parts[i] = parts[i].Trim();
            }

            return removeEmptyEntries ? parts.Where(p => !string.IsNullOrEmpty(p)).ToArray() : parts;
        }

        /// <summary>
        /// Optimized string formatting using StringBuilder for multiple interpolations
        /// </summary>
        /// <param name="template">Template string with {0}, {1}, etc. placeholders</param>
        /// <param name="args">Arguments to substitute</param>
        /// <returns>Formatted string</returns>
        public static string FormatOptimized(string template, params object[] args)
        {
            if (string.IsNullOrEmpty(template))
                return string.Empty;

            if (args == null || args.Length == 0)
                return template;

            try
            {
                return string.Format(template, args);
            }
            catch (FormatException)
            {
                // Fallback to simple concatenation if format fails
                var sb = new StringBuilder(template);
                for (int i = 0; i < args.Length; i++)
                {
                    sb.Replace($"{{{i}}}", args[i]?.ToString() ?? string.Empty);
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Check if string is null, empty, or whitespace efficiently
        /// </summary>
        /// <param name="value">String to check</param>
        /// <returns>True if null, empty, or whitespace</returns>
        public static bool IsNullOrWhiteSpace(string? value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Safe substring operation that doesn't throw exceptions
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="startIndex">Start index</param>
        /// <param name="length">Length (optional)</param>
        /// <returns>Substring or empty string if invalid parameters</returns>
        public static string SafeSubstring(string? input, int startIndex, int? length = null)
        {
            if (string.IsNullOrEmpty(input) || startIndex < 0 || startIndex >= input.Length)
                return string.Empty;

            if (length.HasValue)
            {
                var actualLength = Math.Min(length.Value, input.Length - startIndex);
                return actualLength > 0 ? input.Substring(startIndex, actualLength) : string.Empty;
            }

            return input.Substring(startIndex);
        }
    }
}
