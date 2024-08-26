namespace Core.CoreUtils
{
    public static class Generator
    {
        static IEnumerable<string> SplitCamelCase(string input)
        {
            return System.Text.RegularExpressions.Regex.Split(input, @"(?<!^)(?=[A-Z])");
        }

        static IEnumerable<string> SplitVietnameseWords(string input)
        {
            var vietnameseMarks = new[] { 'à', 'á', 'ả', 'ã', 'ạ', 'â', 'ă', 'đ', 'è', 'é', 'ẻ', 'ẽ', 'ẹ', 'ê',
                                      'ì', 'í', 'ỉ', 'ĩ', 'ị', 'ò', 'ó', 'ỏ', 'õ', 'ọ', 'ô', 'ơ', 'ù', 'ú',
                                      'ủ', 'ũ', 'ụ', 'ư', 'ỳ', 'ý', 'ỷ', 'ỹ', 'ỵ' };

            var words = new List<string>();
            var currentWord = input[0].ToString();

            for (int i = 1; i < input.Length; i++)
            {
                if (vietnameseMarks.Contains(char.ToLower(input[i])) || char.IsUpper(input[i]))
                {
                    words.Add(currentWord);
                    currentWord = "";
                }
                currentWord += input[i];
            }

            if (!string.IsNullOrEmpty(currentWord))
            {
                words.Add(currentWord);
            }

            return words;
        }

    }
}
