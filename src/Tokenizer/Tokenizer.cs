using System.Text.RegularExpressions;
using PyCSS_parser.Common;

namespace PyCSS_parser.Tokenizer;

public class Tokenizer : ITokenizer
{
    public IReadOnlyList<string> TokenizeFile(string fileContent)
    {
        if (string.IsNullOrEmpty(fileContent))
        {
            return new List<string>().AsReadOnly();
        }

        var unifiedFileContent = fileContent.ReplaceLineEndings(Tokens.NewLineCharacter);
        unifiedFileContent = Regex.Replace(unifiedFileContent, @"(\S)?([\,\+\>\<\;])(\S)?", "$1 $2 $3");
        unifiedFileContent = Regex.Replace(unifiedFileContent, @"(\S)?(:)", "$1 $2");

        var tokens = new List<string>();
        var result = Regex.Match(unifiedFileContent, @$"\n|\t|[^\s""']+|""[^""]*""|'[^']*'");
        while (result.Success)
        {
            tokens.Add(result.Value);
            result = result.NextMatch();
        }
        
        PadFileEnding(tokens);
        return tokens.AsReadOnly();
    }

    private static void PadFileEnding(List<string> tokens)
    {
        if (tokens[^1] != Tokens.NewLineCharacter)
        {
            tokens.Add(Tokens.NewLineCharacter);
        }

        if (tokens[^2] != Tokens.NewLineCharacter)
        {
            tokens.Add(Tokens.NewLineCharacter);
        }
    }
}