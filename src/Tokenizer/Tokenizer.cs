using System.Text.RegularExpressions;

namespace PyCSS_parser.Tokenizer;

public class Tokenizer : ITokenizer
{
    public IReadOnlyList<string> TokenizeFile(string fileContent)
    {
        if (string.IsNullOrEmpty(fileContent))
        {
            return new List<string>().AsReadOnly();
        }

        var unifiedFileContent = fileContent.ReplaceLineEndings("\n");
        unifiedFileContent = Regex.Replace(unifiedFileContent, @"(\S)?([\,\+\>\<\:])(\S)?", "$1 $2 $3");
        var tokens = Regex.Split(unifiedFileContent, @" +|(\n)|(\t)").Where(s => s != string.Empty).ToList();
        
        return tokens.AsReadOnly();
    }
}