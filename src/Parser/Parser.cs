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
                    default:
                    {
                        if (!Regexes.Identifier.Match(_tokens[currentTokenIndex]).Success)
                        {
                            throw new InvalidTokenException(_lineNumber,
                                "Oczekiwany identyfikator zgodny z następującym wyrażeniem regularnym:\n" +
                                Regexes.Identifier);
                        }

                        throw new InvalidTokenException(_lineNumber,
                            "Oczekiwany kombinator ze zbioru:\n" +
                            Tokens.Combinators);
                    }
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
            return ParseComment(currentTokenIndex);
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
            }

            if (_tokens[currentTokenIndex] != Tokens.NewLineCharacter)
            {
                continue;
            }

            currentTokenIndex++;
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
        while (_tokens[currentTokenIndex] != Tokens.DeclarationEnding ||
               _tokens[currentTokenIndex] != Tokens.NewLineCharacter)
        {
            
        }
        // while
        // switch

        return currentTokenIndex;
    }
}