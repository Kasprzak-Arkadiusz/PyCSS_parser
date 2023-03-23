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
        foreach (var token in _tokens)
        {
            if (token == Tokens.NewLineCharacter)
            {
                _lineNumber++;
            }
            else if (token == Tokens.CommentBeginning)
            {
                ParseComment();
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

    private static void ParseComment() { }

    private static void ParseClause() { }
}