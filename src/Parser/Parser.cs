using PyCSS_parser.Common;
using PyCSS_parser.Common.Exceptions;

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
                i++;
                i = ParseComment(i);
            }
            else if (Regexes.Identifier.Match(token).Success)
            {
                i++;
                i = ParseClause(i);
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
        while (currentTokenIndex < _tokens.Count - 1)
        {
            if (_tokens[currentTokenIndex] == Tokens.NewLineCharacter)
            {
                _lineNumber++;
                return currentTokenIndex;
            }

            currentTokenIndex++;
        }

        return currentTokenIndex;
    }

    private int ParseClause(int currentTokenIndex)
    {
        currentTokenIndex = ParseClauseHeader(currentTokenIndex);
        currentTokenIndex = ParseClauseBody(currentTokenIndex);
        return currentTokenIndex;
    }

    private int ParseClauseHeader(int currentTokenIndex)
    {
        while (_tokens[currentTokenIndex] != Tokens.NewLineCharacter &&
               _tokens[currentTokenIndex] != Tokens.CommentBeginning)
        {
            var isCombinator = Tokens.Combinators.Contains(_tokens[currentTokenIndex]);
            if (isCombinator)
            {
                currentTokenIndex++;
                switch (_tokens[currentTokenIndex])
                {
                    case Tokens.NewLineCharacter:
                        currentTokenIndex++;
                        _lineNumber++;
                        break;
                    case Tokens.CommentBeginning:
                        currentTokenIndex++;
                        currentTokenIndex = ParseComment(currentTokenIndex);
                        break;
                }
            }

            if (!Regexes.Identifier.Match(_tokens[currentTokenIndex]).Success)
            {
                throw new InvalidTokenException(_lineNumber,
                    "Identyfikator powinien być zgodny z następującym wyrażeniem regularnym:\n" +
                    Regexes.Identifier);
            }

            currentTokenIndex++;
        }

        if (_tokens[currentTokenIndex] == Tokens.CommentBeginning)
        {
            currentTokenIndex++;
            currentTokenIndex = ParseComment(currentTokenIndex);
            currentTokenIndex++;
            return currentTokenIndex;
        }

        _lineNumber++;
        currentTokenIndex++;
        return currentTokenIndex;
    }

    private int ParseClauseBody(int currentTokenIndex)
    {
        while (currentTokenIndex < _tokens.Count - 1)
        {
            if (_tokens[currentTokenIndex] != Tokens.Indent)
            {
                throw new InvalidTokenException(_lineNumber, "Brakujące wcięcie (tabulacja) w ciele wyrażenia");
            }

            currentTokenIndex++;
            currentTokenIndex = ParseExpression(currentTokenIndex);

            if (_tokens[currentTokenIndex] == Tokens.DeclarationEnding)
            {
                currentTokenIndex++;
            }

            if (_tokens[currentTokenIndex] == Tokens.NewLineCharacter)
            {
                currentTokenIndex++;
                _lineNumber++;
            }

            if (_tokens[currentTokenIndex] != Tokens.NewLineCharacter)
            {
                continue;
            }

            _lineNumber++;
            return currentTokenIndex;
        }

        return currentTokenIndex;
    }

    private int ParseExpression(int currentTokenIndex)
    {
        // expression_label
        if (!Regexes.ExpressionLabel.Match(_tokens[currentTokenIndex]).Success)
        {
            throw new InvalidTokenException(_lineNumber, "Niepoprawna etykieta wyrażenia");
        }

        currentTokenIndex++;

        // : 
        if (_tokens[currentTokenIndex] != Tokens.DeclarationLabelSeparator)
        {
            throw new InvalidTokenException(_lineNumber,
                "Niepoprawny token oddzielający etykietę wyrażenia od wartości\n." +
                $" Oczekiwany token to {Tokens.DeclarationLabelSeparator}");
        }

        currentTokenIndex++;

        // expression_value
        while (_tokens[currentTokenIndex] != Tokens.DeclarationEnding &&
               _tokens[currentTokenIndex] != Tokens.NewLineCharacter)
        {
            if (_tokens[currentTokenIndex] == Tokens.Keyword)
            {
                currentTokenIndex++;
            }
            else if (Regexes.UnitValue.Match(_tokens[currentTokenIndex]).Success)
            {
                currentTokenIndex++;
            }
            else if (Regexes.ColorHexValue.Match(_tokens[currentTokenIndex]).Success)
            {
                currentTokenIndex++;
            }
            else if (Regexes.UrlValue.Match(_tokens[currentTokenIndex]).Success)
            {
                currentTokenIndex++;
            }
            else if (Regexes.StringValue.Match(_tokens[currentTokenIndex]).Success)
            {
                currentTokenIndex++;
            }
            else if (Regexes.TextValue.Match(_tokens[currentTokenIndex]).Success)
            {
                currentTokenIndex++;
            }
            else
            {
                throw new InvalidTokenException(_lineNumber,
                    "Niepoprawna wartość wyrażenia. Wartość może być: \n" +
                    "- słowem kluczowym (np. !important)\n" +
                    "- liczbą z jednostką (np. 10rem lub 7in)\n" +
                    "- kolorem zapisanym w formacie heksadecymalnym (np. #fff lub #a01212)\n" +
                    "- adresem url (np. watch?v=Ct6BUPvE2sM\n" +
                    "- napisem (np. \"see below\")\n" +
                    "- tekstem (np. avoid)");
            }
        }

        return currentTokenIndex;
    }
}