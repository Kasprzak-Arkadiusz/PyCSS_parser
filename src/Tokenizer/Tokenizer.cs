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
        unifiedFileContent = Regex.Replace(unifiedFileContent, @"(\S)?([\,\+\>\<\:\;])(\S)?", "$1 $2 $3");
        var tokens = Regex.Split(unifiedFileContent, @$" +|({Tokens.NewLineCharacter})|({Tokens.Indent})")
            .Where(s => s != string.Empty).ToList();

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