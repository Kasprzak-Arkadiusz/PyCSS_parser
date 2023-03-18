using System.Text.RegularExpressions;

namespace PyCSS_parser;

public class Tokenizer : ITokenizer
{
    public IReadOnlyList<string> TokenizeFile(string path)
    {
        var fileContent = File.ReadAllText(path);
        if (string.IsNullOrEmpty(fileContent))
        {
            return new List<string>().AsReadOnly();
        }

        var unifiedFileContent = fileContent.ReplaceLineEndings("\n");
        var tokens = Regex.Split(unifiedFileContent, @"(?<=[ |\n|\t])").ToList();
        SplitNewLineCharacters(tokens);

        return tokens.AsReadOnly();
    }

    private static void SplitNewLineCharacters(IList<string> tokens)
    {
        for (var index = 0; index < tokens.Count; index++)
        {
            var token = tokens[index];
            if (token.Length == 1 || token[^1] != '\n')
            {
                continue;
            }

            tokens[index] = token.TrimEnd('\n');
            tokens.Insert(index + 1, "\n");
            index++;
        }
    }
}