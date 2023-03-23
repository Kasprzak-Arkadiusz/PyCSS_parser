using PyCSS_parser.Common;

namespace PyCSS_parser.Parser;

public class Parser : IParser
{
    private readonly IReadOnlyList<string> _tokens;
    private int _lineNumber;

    public Parser(IReadOnlyList<string> tokens)
    {
        _tokens = tokens;
        _lineNumber = 1;
    }

    public void Parse()
    {
        for (var i = 0; i < _tokens.Count; i++)
        {
            var token = _tokens[i];
            if (token == Tokens.NewLineCharacter)
            {
                _lineNumber++;
            }
            else if (token == Tokens.CommentBeginning)
            {
                i = ParseComment(i);
            }
            else if (Regexes.Identifier.Match(token).Success)
            {
                ParseClause();
            }
            else
            {
                throw new TokenNotDefinedException(
                    $"Analizowany token nie został uwzględniony w gramatyce: {token}");
            }
        }
    }

    private int ParseComment(int currentTokenIndex)
    {
        currentTokenIndex++;
        while (currentTokenIndex < _tokens.Count)
        {
            if (_tokens[currentTokenIndex] == Tokens.NewLineCharacter)
            {
                return currentTokenIndex;
            }

            currentTokenIndex++;
        }

        return currentTokenIndex;
    }

    private static void ParseClause() { }
}